using Framework;
using SFML.Graphics;
using SFML.Window;

namespace LD30
{
    public class GameOverState : GameState
    {
        private Game _game;
        private Text _text;

        public GameOverState(Game g)
        {
            _game = g;
        }

        public override void OnEnter()
        {
            base.OnEnter();
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
        }

        public override void OnUpdate(int delta)
        {
            base.OnUpdate(delta);
        }

        public override void OnRender(RenderTarget rt)
        {
            base.OnRender(rt);

            _game.playState.world.Render(rt);

            // Switch to interface view
            rt.SetView(rt.DefaultView);

            rt.Draw(_text);
			_game.playState.score.Render(rt);
        }

        public override void OnKeyPressed(KeyEventArgs e)
        {
        }
    }
}
