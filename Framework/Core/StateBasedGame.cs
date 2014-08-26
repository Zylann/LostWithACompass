using SFML.Graphics;
using SFML.Window;

namespace Framework
{
	public abstract class StateBasedGame : Game
	{
		private GameState _currentState;

		public StateBasedGame(string title) : base(title)
		{
		}

		public void EnterState(GameState state)
		{
			if (_currentState != null)
			{
				_currentState.OnLeave();
			}

			_currentState = state;
			_currentState.OnEnter();
		}

		public override void Init()
		{
		}

		public override void Update(int delta)
		{
			_currentState.OnUpdate(delta);
		}

		public override void Render(RenderTarget rt)
		{
			_currentState.OnRender(rt);
		}

		public override void Quit()
		{
			_currentState.OnLeave();
		}

		public override void OnScreenResized(Vector2u newSize)
		{
			_currentState.OnScreenResized(newSize);
		}

		public override void OnKeyPressed(KeyEventArgs e)
		{
			_currentState.OnKeyPressed(e);
		}

		public override void OnMouseButtonPressed(MouseButtonEventArgs e)
		{
			_currentState.OnMouseButtonPressed(e);
		}

		public override void OnMouseButtonReleased(MouseButtonEventArgs e)
		{
			_currentState.OnMouseButtonReleased(e);
		}
	}
}

