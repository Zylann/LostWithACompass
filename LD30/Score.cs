using Framework;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
	public class Score
	{
		private int _value;
		private Text _text;

		public void Init()
		{
			_text = new Text("Score: 0", Game.font);
			_text.CharacterSize = 8;
			_text.Color = new Color(255, 128, 0);
			UpdatePosition();
		}

		public void Update(int value)
		{
			_value = value;
			_text.DisplayedString = "Score: " + _value;
		}

		public void UpdatePosition()
		{
			Vector2u ss = Application.instance.ScreenSize;
			_text.Position = new Vector2f(ss.X - 80, 4);
		}

		public void Render(RenderTarget rt)
		{
			rt.Draw(_text);
		}
	}
}


