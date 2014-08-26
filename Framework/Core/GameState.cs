using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Framework
{
	public abstract class GameState
	{
		public virtual void OnEnter() { }
		public virtual void OnLeave() { }

		public virtual void OnInit() { }
		public virtual void OnUpdate(int delta) { }
		public virtual void OnRender(RenderTarget rt) { }

		public virtual void OnScreenResized(Vector2u newSize) { }

		public abstract void OnKeyPressed(KeyEventArgs e);

		public virtual void OnMouseButtonPressed(MouseButtonEventArgs e) { }
		public virtual void OnMouseButtonReleased(MouseButtonEventArgs e) { }

	}
}



