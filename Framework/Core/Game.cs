using SFML.Graphics;
using SFML.Window;

namespace Framework
{
	public abstract class Game
	{
		public Application application;
		private string _title;

		public Game(string title)
		{
			_title = title;
		}

		public string title
		{
			get { return _title; }
		}

		public abstract void Init();
		public abstract void Update(int delta);
		public abstract void Render(RenderTarget rt);
		public abstract void Quit();

		public abstract void OnScreenResized(Vector2u newSize);

		public abstract void OnKeyPressed(KeyEventArgs e);

		public abstract void OnMouseButtonPressed(MouseButtonEventArgs e);
		public abstract void OnMouseButtonReleased(MouseButtonEventArgs e);

	}
}

