using Framework;
using SFML.Graphics;
using SFML.Window;

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

		private float _titleAlpha = 0;

		public MenuState(Game g)
		{
			_game = g;
		}

		public override void OnInit()
		{
			_title = new Text("LOST\nWITH A COMPASS", Game.font);
			_title.Position = new Vector2f(8, 4);
			_title.CharacterSize = 32;
			_title.Color = new Color(64, 128, 0);

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
			_text = new Text("", Game.font);
			_text.Position = new Vector2f(16, 100);
			_text.CharacterSize = 12;
			_text.Color = new Color(128, 128, 128);

			_me = new Text("A game by Marc Gilleron @ZylannMP3\nFor Ludum Dare 30 compo #ld48\n"+Application.instance.version, Game.font);
			_me.Color = new Color(64, 128, 0);
			_me.CharacterSize = 16;
		}

		public override void OnUpdate(int delta)
		{
			_ticks++;
			if (_ticks % 2 == 0)
			{
				_displayIndex++;
				if (_displayIndex > _introText.Length)
				{
					_displayIndex = _introText.Length;
					_titleAlpha = System.Math.Min(_titleAlpha + (float)delta / 2000f, 1f);
				}
				else
					_text.DisplayedString += _introText[_displayIndex - 1];
			}
		}

		public override void OnRender(RenderTarget rt)
		{
			//rt.Clear(Color.Black);

			_title.Color = new Color(_title.Color.R, _title.Color.G, _title.Color.B, (byte)(255f * _titleAlpha));
			_me.Color = new Color(_me.Color.R, _me.Color.G, _me.Color.B, (byte)(255f * _titleAlpha));

			rt.Draw(_title);
			rt.Draw(_text);

			_me.Position = new Vector2f(16, Application.instance.ScreenSize.Y - 70);
			rt.Draw(_me);

		}

		public override void OnMouseButtonPressed(MouseButtonEventArgs e)
		{
			if (_ticks > 80 && Mouse.IsButtonPressed(Mouse.Button.Left))
			{
				if(_displayIndex < _introText.Length)
				{
					_displayIndex = _introText.Length;
					_text.DisplayedString = _introText;
				}
				else
				{
					_game.EnterState(_game.playState);
				}
			}
		}

		public override void OnKeyPressed(KeyEventArgs e)
		{
			if(_ticks > 200)
				_game.EnterState(_game.playState);
		}
	}
}

