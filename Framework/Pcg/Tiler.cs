using System;
using System.Collections.Generic;

namespace Framework
{
	public enum TilerPatternType
	{
		// TilerPattern.defaultOutput is used.
		// - - -
		// - o -
		// - - -
		SINGLE = 0,

		// TilerPattern.tiles are in bitmask order.
		// - - -   - - -   - - -   - - -
		// - o o   o o o   o o -   - o -
		// - o -   - o -   - o -   - o -
		//
		// - o -   - o -   - o -   - o -
		// - o o   o o o   o o -   - o -
		// - o -   - o -   - o -   - o -
		//
		// - o -   - o -   - o -   - o -
		// - o o   o o o   o o -   - o -
		// - - -   - - -   - - -   - - -
		//
		// - - -   - - -   - - -   - - -
		// - o o   o o o   o o -   - o -
		// - - -   - - -   - - -   - - -
		PATH = 1,

		// TilerPattern.tiles are in right-read order of this scheme.
		// o o o   o o o   o o o   - - x   x - -
		// o o o   o o o   o o o   - o o   o o -
		// o o -   x - x   - o o   x o o   o o x
		// 
		// o o x   - - -   x o o   x o o   o o x
		// o o -   - - -   - o o   - o o   o o -
		// o o x   - - -   x o o   - - x   x - -
		// 
		// o o -   x - x   - o o   o o o   - - -
		// o o o   o o o   o o o   o o o   - - -
		// o o o   o o o   o o o   o o o   - - -
		BORDER = 2
	}

	public class TilerPattern
	{
		public int inputID;
		public TilerPatternType type = TilerPatternType.SINGLE;
		public int[] defaultOutput; // Will be picked at random
		public List<bool> connects = new List<bool>();
		public int[] tiles;

		public TilerPattern(int inputID, int singleOutput)
		{
			this.inputID = inputID;
			type = TilerPatternType.SINGLE;
			defaultOutput = new int[1]{singleOutput};
		}

		public TilerPattern(int inputID, int[] singleRandomOutput)
		{
			this.inputID = inputID;
			type = TilerPatternType.SINGLE;
			defaultOutput = singleRandomOutput;
		}

		public TilerPattern(int inputID, TilerPatternType type, int[] tiles, int[] connectsWith = null)
		{
			this.inputID = inputID;
			this.type = type;
			this.tiles = tiles;

			if(connectsWith != null)
			{
				for(int i = 0; i < connectsWith.Length; ++i)
				{
					AddConnect(connectsWith[i]);
				}
			}

			// Self-connect
			if (type == TilerPatternType.BORDER || type == TilerPatternType.PATH)
			{
				AddConnect(inputID);
			}
		}

		public void AddConnect(int tileID)
		{
			while(connects.Count <= tileID)
			{
				connects.Add(false);
			}
			connects[tileID] = true;
		}

		public bool Connects(int tileID)
		{
			if (tileID < 0 || tileID >= connects.Count)
				return false;
			return connects[tileID];
		}
	}

	public class Tiler
	{
		public int[] mapInput; // [input] = mapped input (tileType)
		public List<TilerPattern> patterns = new List<TilerPattern>();
		public int defaultOutput = -1;
		public int defaultInput = -1;
		private int[] _borderLookup;

		public Tiler()
		{
			CompileBorderLookup();
		}

		public void AddPattern(TilerPattern pattern)
		{
			while(patterns.Count-1 < pattern.inputID)
			{
				patterns.Add(null);
			}
			patterns[pattern.inputID] = pattern;
		}

		private void CompileBorderLookup()
		{
			// Human-readable cases in right-read order (see BORDER description above).
			// Each character has this order :
			// 0 1 2
			// 3   4
			// 5 6 7
			string[] cases = new string[] {
				"11111110",
				"11111*0*",
				"11111011",
				"00*01*11",
				"*001011*",

				"11*1011*",
				"00000000",
				"*1101*11",
				"*110100*",
				"11*10*00",

				"11011111",
				"*0*11111",
				"01111111",
				"11111111"
			};

			CompileBorderLookup(cases);
		}

		private void CompileBorderLookup(string[] cases)
		{
			_borderLookup = new int[256];

			for (int caseNumber = 0; caseNumber < cases.Length; ++caseNumber)
			{
				// Get static and dontCare bits from string description.
				// The static part are bits that must have the specified value,
				// The dontCare part are bits that can be 0 or 1.

				string caseString = cases[caseNumber];

				int staticMask = 0;
				int dontCareMask = 0;

				for (int i = 0; i < caseString.Length; ++i)
				{
					char c = caseString[i];
					switch(c)
					{
						case '1':
							staticMask |= (1 << i);
							break;

						case '*':
							dontCareMask |= (1 << i);
							break;

						default:
							break;
					}
				}

				// Count set bits in the dontCareMask and memorize their position
				int[] variableBits = new int[8]; // 1-bit masks representing the Nth variable bit
				int variableBitCount = 0; // How many bits are variable
				for(int i = 0; i < 8; ++i)
				{
					int bit = (1 << i);
					if((dontCareMask & bit) != 0)
					{
						variableBits[variableBitCount++] = bit;
					}
				}

				// Generate possible neighborings :
				// Use a number going from 0 to max, and dispatch its bits in the final
				// neighboring where bits are variable.
				//---------
				// Example
				//---------
				// Some case:           0b10100100
				//                          +
				// Mask:                0b00011010
				//                          =
				// Means:               0b101**1*0 <-- as in the string representation
				//
				// Needed combinations: 0b10100100  n = 0b000 (0)
				//                           ^^ ^
				//                      0b10100110  n = 0b001 (1)
				//                           ^^ ^
				//                      0b10101100  n = 0b010 (2)
				//                           ^^ ^
				//                      0b10101110  n = 0b011 (3)
				//                      ...
				//                      0b10111110  n = 0b111 (7)
				//                           ^^ ^
				int max = 1 << variableBitCount; // There is 2^variableBitCount combinations
				for(int n = 0; n < max; ++n) // For each combination number
				{
					int generatedMask = staticMask;
					for(int i = 0; i < variableBitCount; ++i)
					{
						int bit = (1 << i);
						if((n & bit) != 0)
						{
							generatedMask |= variableBits[i];
						}
					}

					_borderLookup[generatedMask] = caseNumber;
				}
			}
		}

		public void Process(Array2D<int> input, Array2D<int> output)
		{
			for(int y = 0; y < input.sizeY; ++y)
			{
				for(int x = 0; x < input.sizeX; ++x)
				{
					output[x,y] = ProcessTile(input, x, y);
				}
			}
		}

		public int ProcessTile(Array2D<int> input, int x, int y)
		{
			int t = input[x, y];
			if (t < 0)
			{
				return defaultOutput;
			}

			int tt = mapInput != null ? mapInput[t] : t;
			TilerPattern p = patterns[tt];

			if(p == null)
			{
				return defaultOutput;
			}

			switch(p.type)
			{
				case TilerPatternType.PATH:
					return ProcessTilePath(input, x, y, tt);

				case TilerPatternType.BORDER:
					return ProcessTileBorder(input, x, y, tt);

				default: // SINGLE
					return p.defaultOutput[Random.Range(0, p.defaultOutput.Length)];
			}
		}

		private int ProcessTilePath(Array2D<int> input, int x, int y, int tt)
		{
			TilerPattern p = patterns[tt];
			return p.tiles[Get4DirMask(input, x, y, p)];
		}

		private int ProcessTileBorder(Array2D<int> input, int x, int y, int tt)
		{
			TilerPattern p = patterns[tt];
			int m = Get8DirMask(input, x, y, p);
			int l = _borderLookup[m];
			return p.tiles[l];
		}

		private int GetCell(Array2D<int> cells, int x, int y)
		{
			if(cells.Contains(x,y))
			{
				int c = cells[x, y];
				if(mapInput != null)
					return mapInput[cells[x, y]];
				return c;
			}
			else
			{
				return defaultInput;
			}
		}

		private int Get4DirMask(Array2D<int> input, int x, int y, TilerPattern p)
		{
			//   3
			// 0   1
			//   2

			int m = 0;

			if (p.Connects(GetCell(input, x - 1, y))) m |= 1;
			if (p.Connects(GetCell(input, x + 1, y))) m |= 2;
			if (p.Connects(GetCell(input, x, y - 1))) m |= 4;
			if (p.Connects(GetCell(input, x, y + 1))) m |= 8;

			//if (x != 0 && p.Connects(mapInput[input[x - 1, y]])) m |= 1;
			//if (y != 0 && p.Connects(mapInput[input[x, y - 1]])) m |= 4;
			//if (x != input.sizeX - 1 && p.Connects(input[x + 1, y])) m |= 2;
			//if (y != input.sizeY - 1 && p.Connects(input[x, y - 1])) m |= 8;

			return m;
		}

		private int Get8DirMask(Array2D<int> input, int x, int y, TilerPattern p)
		{
			// 0 1 2
			// 3   4
			// 5 6 7

			int m = 0;

			if (p.Connects(GetCell(input, x - 1, y - 1))) m |= 1;
			if (p.Connects(GetCell(input, x,     y - 1))) m |= 2;
			if (p.Connects(GetCell(input, x + 1, y - 1))) m |= 4;
			if (p.Connects(GetCell(input, x - 1, y    ))) m |= 8;
			if (p.Connects(GetCell(input, x + 1, y    ))) m |= 16;
			if (p.Connects(GetCell(input, x - 1, y + 1))) m |= 32;
			if (p.Connects(GetCell(input, x,     y + 1))) m |= 64;
			if (p.Connects(GetCell(input, x + 1, y + 1))) m |= 128;

			//if (x != 0)
			//{
			//	if (y != 0 && p.Connects(mapInput[input[x - 1, y - 1]]))
			//		m |= (1 << 0);
			//	if (y != input.sizeY - 1 && p.Connects(mapInput[input[x - 1, y + 1]]))
			//		m |= (1 << 5);
			//	if (p.Connects(mapInput[input[x - 1, y]]))
			//		m |= (1 << 3);
			//}

			//if (x != input.sizeX - 1)
			//{
			//	if (y != 0 && p.Connects(mapInput[input[x + 1, y - 1]]))
			//		m |= (1 << 2);
			//	if (y != input.sizeY - 1 && p.Connects(mapInput[input[x + 1, y + 1]]))
			//		m |= (1 << 7);
			//	if (p.Connects(mapInput[input[x + 1, y]]))
			//		m |= (1 << 4);
			//}

			//if (y != 0 && p.Connects(mapInput[input[x, y - 1]]))
			//	m |= (1 << 1);
			//if (y != input.sizeY - 1 && p.Connects(mapInput[input[x, y + 1]]))
			//	m |= (1 << 6);

			return m;
		}
		
	}
}

