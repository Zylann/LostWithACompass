using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
	public class DebugOverlay
	{
		private static DebugOverlay __instance;
		public static DebugOverlay instance
		{
			get
			{
				if (__instance == null)
					__instance = new DebugOverlay();
				return __instance;
			}
		}

		private Dictionary<string, string> _entries = new Dictionary<string,string>();
		private Text _text = new Text();

		private DebugOverlay()
		{
			_text.Color = Color.White;
		}

		public void SetFont(Font font, uint charSize)
		{
			_text.Font = font;
			_text.CharacterSize = charSize;
		}

		public DebugOverlay Line(string label, object value)
		{
			if(_entries.ContainsKey(label))
			{
				_entries[label] = value.ToString();
			}
			else
			{
				_entries.Add(label, value.ToString());
			}

			return this;
		}

		public void Render(RenderTarget rt)
		{
			string str = "";
			foreach(KeyValuePair<string,string> pair in _entries)
			{
				str += pair.Key + ": " + pair.Value + "\n";
			}

			_text.DisplayedString = str;

			rt.Draw(_text);
		}
	}
}

