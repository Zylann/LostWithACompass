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
	public class MenuState : GameState
	{
		private Text _title;
		private Text _text;

		private string _introText;
		private int _displayIndex;
		private int _ticks;

		private Text _me;

		private Game _game;

		public MenuState(Game g)
		{
			_game = g;
		}

		public override void OnInit()
		{
			_title = new Text("LOST\nWITH A COMPASS", Game.font);
			_title.Position = new Vector2f(8, 4);
			_title.CharacterSize = 24;
			_title.Color = new Color(255, 128, 0);

			// YOU SEE THE HACK ?
			// DIIIIIIIIIIRTY
			_introText = "They say this place is haunted.                                                                \n"
				+ "People often disappear without ever coming back.                                                      \n"
				+ "They say something has connected this place to hell.                                                                                       \n"
				+ "Legends talk about a bloody world where you can't stand alive,\n"
				+ "and ghosts that cannot be stopped.                                                     \n"
				+ "\n"
				+ "You came here with your grandfather's compass.                                                    \n"
				+ "It never indicates North, but this forest...                                                       \n"
				+ "You came here to verify what's wrong.                                                         \n"
				+ "                                                                     \n"
				+ "Then you got lost.\n";
			_text = new Text("_", Game.font);
			_text.Position = new Vector2f(16, 80);
			_text.CharacterSize = 8;
			_text.Color = new Color(128, 128, 128);

			_me = new Text("A game by Marc Gilleron @ZylannMP3 // For Ludum Dare 30 compo #ld48\nAlpha version 1.0", Game.font);
			_me.Color = new Color(128, 64, 0);
			_me.CharacterSize = 8;
		}

		public override void OnRender(RenderTarget rt)
		{
			//rt.Clear(Color.Black);

			_ticks++;
			if(_ticks%2==0)
			{
				_displayIndex++;
				if (_displayIndex > _introText.Length)
					_displayIndex = _introText.Length;
				else
					_text.DisplayedString += _introText[_displayIndex - 1];
			}

			rt.Draw(_title);
			rt.Draw(_text);

			_me.Position = new Vector2f(4, Application.instance.ScreenSize.Y - 25);
			rt.Draw(_me);

			if(_ticks > 200 && Mouse.IsButtonPressed(Mouse.Button.Left))
			{
				_game.EnterState(_game.playState);
			}
		}

		public override void OnKeyPressed(KeyEventArgs e)
		{
			if(_ticks > 200)
				_game.EnterState(_game.playState);
		}
	}
}

