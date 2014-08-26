using System;
using SFML.Graphics;
using SFML.Window;
using SFML.Audio;
using System.IO;

namespace Framework
{
	public class Application
	{
		public static Application instance { get { return __instance; } }
		private static Application __instance;

		private RenderWindow _window;
		private RenderTexture _rt; // Downscalable game framebuffer
		private VertexArray _rtQuad;
		private View _view;
		private Sprite _cursor;
		public Color clearColor = new Color(8, 12, 14);

		private ApplicationSettings _settings;

		private Game _game;
		private bool _running = false;

		private DebugStat _debugDelta;

		private TimeStepper _timeStepper;

		public AppVersion version = new AppVersion();

		public Application(Game game)
		{
			if (__instance == null)
				__instance = this;
			else
				throw new Exception("Can't create two Applications");

			_settings = new ApplicationSettings();
			_game = game;
			_game.application = this;

			int span = _settings.maxDeltaMs - _settings.minDeltaMs;
			_debugDelta = new DebugStat(100, 30,
				_settings.minDeltaMs - span/8,
				_settings.minDeltaMs + span/8,
				new Vector2f(220f, 2f)
			);
		}

		public Vector2f MapWindowToVirtualScreen(Vector2i mousePos)
		{
			Vector2f pos = _window.MapPixelToCoords(mousePos);
			pos.X = Mathf.Floor(pos.X);
			pos.Y = Mathf.Floor(pos.Y);
			return pos;
		}

        public Vector2f cursorPosition
        {
            get { return _cursor.Position; }
        }

		public Vector2u ScreenSize
		{
			get { return _rt.Size; }
		}

		public void Start()
		{
			if (_running)
				return;
			_running = true;

			if (_settings.debug)
			{
				// Create debug directory if it doesn't exist
				if (!Directory.Exists("debug_data"))
				{
					Directory.CreateDirectory("debug_data");
				}
			}

			// Create window
			_window = new RenderWindow(new VideoMode(1200, 800), _game.title);
			_window.SetFramerateLimit(60);
			_window.SetVerticalSyncEnabled(_settings.vsync);
			_window.SetMouseCursorVisible(_settings.showSystemCursor);
			_window.SetActive(true);

			UpdateRenderTarget();

			_view = new View(_window.GetView());
			_view.Size = _view.Size / _settings.pixelSize;
			_view.Center = _view.Size / 2f;
			_window.SetView(_view);

			// Statically load assets
			Assets.Load();

			// Register input events
			_window.Closed += OnClosed;
			_window.KeyPressed += OnKeyPressed;
			_window.Resized += OnWindowResized;
			_window.MouseButtonPressed += OnMouseButtonPressed;
			_window.MouseButtonReleased += OnMouseButtonReleased;

			// Initialize game
			_game.Init();

			_timeStepper = new TimeStepper();
			_timeStepper.SetDeltaRange(_settings.minDeltaMs, _settings.maxDeltaMs);

			// Main loop
			while(_window.IsOpen() && _running)
			{
				_timeStepper.OnBeginFrame();

				int timeBefore = Environment.TickCount;

				// Send input events
				_window.DispatchEvents();

				UpdateGame();

				Render();

				_timeStepper.OnEndFrame();
			}

			_game.Quit();

			// Ensure the window is closed
			if (_window.IsOpen())
				_window.Close();

			_running = false;
		}

		private void UpdateGame()
		{
			_timeStepper.CallSteps(_game.Update);
		}

		private void Render()
		{
			// Render the game
			_rt.Clear(clearColor);

			_game.Render(_rt);

			if (_cursor != null)
			{
				_cursor.Position = MapWindowToVirtualScreen(Mouse.GetPosition(_window));
				_rt.SetView(_rt.DefaultView);
				_rt.Draw(_cursor);
			}

			if (_settings.debug && Keyboard.IsKeyPressed(Keyboard.Key.F3))
			{
				// Display debug informations
				DrawDebugOverlay(_rt);
			}

			_rt.Display();

			// Render the game frame into the window
			_window.Clear(Color.Black);
			_window.Draw(_rtQuad, new RenderStates(_rt.Texture));

			// Flip screen
			_window.Display();
		}

		private void DrawDebugOverlay(RenderTarget rt)
		{
			Vector3f listenerPos = Listener.Position;

			DebugOverlay.instance.Line("FPS (recorded)", _timeStepper.RecordedFPS);
			DebugOverlay.instance.Line("FPS (instant)", (int)(_timeStepper.rawDelta != 0 ? 1000f / (float)_timeStepper.rawDelta : 0));
			DebugOverlay.instance.Line("Delta (instant)", _timeStepper.rawDelta);
			DebugOverlay.instance.Line("Audio listener", (int)listenerPos.X + ", " + (int)listenerPos.Y + ", " + (int)listenerPos.Z);
			DebugOverlay.instance.Line("Audio sources", AudioSystem.instance.GetUsedSourcesCount() + "/" + AudioSystem.instance.maxSources);
			DebugOverlay.instance.Render(rt);

			_debugDelta.Push(_timeStepper.rawDelta);
			rt.Draw(_debugDelta);
		}

		public RenderWindow window
		{
			get { return _window; }
		}

		public void SetCursor(Texture texture)
		{
			if(_cursor == null)
			{
				_cursor = new Sprite();
			}

			_cursor.Texture = texture;
		}

		public int timeMs
		{
			get { return _timeStepper.time; }
		}

		private void UpdateRenderTarget()
		{
			if (_rt == null || !_settings.fixedSize)
			{
				_rt = new RenderTexture(
					_window.Size.X / _settings.pixelSize,
					_window.Size.Y / _settings.pixelSize,
					false
				);
				_rt.Smooth = false;
			}

			_rt.Clear(clearColor);
			_rt.Display();

			if (_rtQuad == null)
			{
				_rtQuad = new VertexArray(PrimitiveType.Quads, 4);
			}

			Vector2f size = new Vector2f(_rt.Size.X, _rt.Size.Y);

			Vertex v = new Vertex();
			v.Color = Color.White;

			_rtQuad[0] = v;

			v.Position.X += size.X;
			v.TexCoords.X += size.X;
			_rtQuad[1] = v;

			v.Position.Y += size.Y;
			v.TexCoords.Y += size.Y;
			_rtQuad[2] = v;

			v.Position.X -= size.X;
			v.TexCoords.X -= size.X;
			_rtQuad[3] = v;
		}

		#region events

		private void OnClosed(object sender, EventArgs e)
		{
			// Close the window when the close button is pressed
			_window.Close();
		}

		private void OnKeyPressed(object sender, KeyEventArgs e)
		{
			if ((e.Code == Keyboard.Key.F4 && e.Alt) || e.Code == Keyboard.Key.Escape)
			{
				_window.Close();
			}
			else
			{
				_game.OnKeyPressed(e);
			}
		}

		private void OnWindowResized(object sender, SizeEventArgs e)
		{
			UpdateRenderTarget();

			if(_settings.pixelPerfect)
			{
				_view.Center = (_rtQuad[0].Position + _rtQuad[2].Position) / 2f;

				_view.Size = new Vector2f(
					_window.Size.X/_settings.pixelSize,
					_window.Size.Y/_settings.pixelSize
				);

				_window.SetView(_view);
			}

			_game.OnScreenResized(_rt.Size);
		}

		public void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
		{
			Log.Debug(e.X + ", " + e.Y);
			_game.OnMouseButtonPressed(e);
		}

		public void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
		{
			_game.OnMouseButtonReleased(e);
		}

		#endregion
	}
}


