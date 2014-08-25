using Framework;
using LD30.Actors;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LD30
{
    public class PlayState : GameState
    {
        private Game _game;
        //private View _worldView;
        private RectangleShape _overlay;
        private Sound _acidSound; // That's not really acid, more like "trans-dimensional mind tearing causing health points to go down"
        public LDWorld world;
        public HealthBar healthBar;
        public Compass compass;
		public Score score;

        public PlayState(Game game)
        {
            _game = game;
        }

        public override void OnInit()
        {
            world = new LDWorld();

            world.lightSystem = new LightSystem(world, _game.application.ScreenSize);

            Texture overlayTexture = Assets.textures["vignette"];
            overlayTexture.Smooth = true;
            _overlay = new RectangleShape();
            _overlay.Texture = overlayTexture;

            MainCamera camera = world.Spawn<MainCamera>();
            world.map = world.SpawnEntity().AddComponent<Map>();

            healthBar = new HealthBar();
            healthBar.Init();

            compass = new Compass();
            compass.Init();

			score = new Score();
			score.Init();
        }

        /// <summary>
        /// THAT WAS A BAD IDEA
        /// IF YOU HAVE TIME IT'S A LOT OF CODING FUN
        /// NOT IN GAME JAMS
        /// (anyway I'll try to fix issues by modifying my game design)
        /// </summary>
        private void GenerateWorld()
        {
            world.map.Generate();

            // Avatar
            Avatar avatar = world.SpawnEntity().AddComponent<Avatar>();
            Vector2i avatarPos = world.FindSpawnableZone();
            avatar.entity.body.position = new Vector2f((float)avatarPos.X + 0.5f, (float)avatarPos.Y + 0.5f);

            // Enemies
            int enemyCount = 12;
            for(int i = 0; i < enemyCount; ++i)
            {
                Entity daemon = world.SpawnEntity().AddComponent<Daemon>().entity;
                daemon.body.position = new Vector2f(
                    Random.Range(0, world.map.cells.sizeX),
                    Random.Range(0, world.map.cells.sizeY)
                );
            }

            // pixies
            int pixieCount = 24;
            for(int i = 0; i < pixieCount; ++i)
            {
                world.SpawnEntity().AddComponent<Pixie>();
            }

            // Portals far enough from the player
            int portalCount = 6;
            for (int i = 0; i < portalCount; ++i)
            {
                Portal portal = world.SpawnEntity().AddComponent<Portal>();
                portal.Reposition();
            }

            // TEST
            //Entity portal2 = world.SpawnEntity().AddComponent<Portal>().entity;
            //portal2.body.position = new Vector2f(avatarPos.X+4, avatarPos.Y);

            world.mainCamera.entity.GetComponent<CameraHandler>().target = avatar.entity;
        }

        public override void OnEnter()
        {
            _acidSound = AudioSystem.instance.Play(Assets.soundBuffers["acid"], 0.2f);
            _acidSound.Loop = true;

            GenerateWorld();
        }

        public override void OnLeave()
        {
            _acidSound.Stop();
        }

        public float CalculateCompassAngle()
        {
            // TODO make the compass crazy for a small amount of time when we cross worlds

            Vector2f avatarPos = world.avatar.body.position;

            IEnumerable<Entity> portals = world.taggedEntities["Portal"];
            float smallestDistance = 999999f;
            Vector2f nearestPortalPos = new Vector2f(0,0);
            foreach(Entity p in portals)
            {
                Vector2f ppos = p.body.position;
                float d = ppos.DistanceTo(avatarPos);
                if(d < smallestDistance)
                {
                    smallestDistance = d;
                    nearestPortalPos = ppos;
                }
            }

            return Mathf.RAD2DEG * ((nearestPortalPos - avatarPos).Angle());
        }

        public override void OnUpdate(int delta)
        {
            world.Update(delta);

			Avatar avatar = world.avatar.GetComponent<Avatar>();

			float acidVol = 0.5f*world.mainCamera.entity.GetComponent<CameraHandler>().shakeAmplitude;
			if (world.dimension == 1)
				acidVol += 0.1f;
			//DebugOverlay.instance.Line("Cracks vol", acidVol);
			_acidSound.Volume = 100f*Mathf.Clamp01(acidVol);

            healthBar.Update(avatar.hp);
            compass.Update(CalculateCompassAngle());
			score.Update(Game.score);

            if (world.avatar.GetComponent<Avatar>().IsDead())
            {
                _game.EnterState(_game.gameOverState);
            }
        }

        public override void OnRender(RenderTarget rt)
        {
            // Render the world
            world.Render(rt);

            // Switch to interface view
            rt.SetView(rt.DefaultView);
            // Draw overlay
            //_overlay.Size = new Vector2f(rt.Size.X, rt.Size.Y);
            rt.Draw(_overlay);

            healthBar.Render(rt);
            compass.Render(rt);
			score.Render(rt);
        }

        public override void OnScreenResized(Vector2u newSize)
        {
            world.OnScreenResized(newSize);
            compass.UpdatePosition();
			score.UpdatePosition();
        }

        public override void OnKeyPressed(KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.M)
            {
                world.SetDimension(1);
            }
            else if (e.Code == Keyboard.Key.P)
            {
                world.SetDimension(0);
            }
            else if(e.Code == Keyboard.Key.Num9)
            {
                Array2DImager<int> imager = new Array2DImager<int>();
                imager.MapColor(0, Color.Black);
                imager.MapColor(1, Color.White);
                imager.MapColor(2, Color.Red);
                imager.MapColor(3, Color.Yellow);
                imager.CreateAndSaveImage(world.map.cells, "map.png");
            }
        }
    }
}
