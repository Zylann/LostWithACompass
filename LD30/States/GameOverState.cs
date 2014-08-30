using Framework;
using SFML.Graphics;
using SFML.Window;

namespace LD30
{
    public class GameOverState : GameState
    {
        private Game _game;
        private Text _text;
        private int _timeBeforeRestartAllowed;
        private Text _restartText;

        public GameOverState(Game g)
        {
            _game = g;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _timeBeforeRestartAllowed = 1000;
            Log.Debug("Game over");
            AudioSystem.instance.Play(Assets.soundBuffers["death"], 0.5f);
        }

        public override void OnInit()
        {
            base.OnInit();
            
            _text = new Text("GAME OVER", Game.font);
            _text.Position = new Vector2f(20, 20);
            _text.CharacterSize = 32;
            _text.Color = new Color(255, 128, 0);

            _restartText = new Text("Click to restart", Game.font);
            _restartText.Position = _text.Position + new Vector2f(0f, 40f);
            _restartText.CharacterSize = 8;
            _restartText.Color = _text.Color;
        }

        public override void OnUpdate(int delta)
        {
            base.OnUpdate(delta);

            _timeBeforeRestartAllowed -= delta;
        }

        public override void OnRender(RenderTarget rt)
        {
            base.OnRender(rt);

            _game.playState.world.Render(rt);

            // Switch to interface view
            rt.SetView(rt.DefaultView);

            rt.Draw(_text);
			_game.playState.score.Render(rt);
            if(_timeBeforeRestartAllowed < 0)
            {
                rt.Draw(_restartText);
            }
        }

        public override void OnMouseButtonPressed(MouseButtonEventArgs e)
        {
            if(_timeBeforeRestartAllowed < 0)
            {
                _game.EnterState(_game.playState);
            }
        }

        public override void OnKeyPressed(KeyEventArgs e)
        {
        }
    }
}
