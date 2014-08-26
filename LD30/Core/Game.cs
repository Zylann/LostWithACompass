using Framework;
using SFML.Audio;
using SFML.Graphics;

namespace LD30
{
    public class Game : Framework.StateBasedGame
    {
        /// <summary>
        /// Basic size of a tile in pixels
        /// </summary>
        public const int TS = 32;

        public static Font font;
        public static Font fontBold;

        public static string DEBUG_DIR = "debug_data";

        public PlayState playState;
        public GameOverState gameOverState;
		public MenuState menuState;

        public Music music;

		public static int score;

        public Game()
            : base("Lost with a compass - Ludum Dare 30")
        {
        }

        public override void Init()
        {
            font = Assets.fonts["uni05_53"];
            fontBold = Assets.fonts["uni05_63"];

            application.SetCursor(Assets.textures["cursor"]);

            DebugOverlay.instance.SetFont(font, 8);

            playState = new PlayState(this);
            gameOverState = new GameOverState(this);
			menuState = new MenuState(this);

            music = new Music(Assets.streamedSounds["music1_loop"]);
            music.Play();
            music.Loop = true;

			menuState.OnInit();
            playState.OnInit();
            gameOverState.OnInit();

            EnterState(menuState);
        }
    }
}
