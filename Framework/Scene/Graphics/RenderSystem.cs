using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;

namespace Framework
{
	public class RenderSystem
	{
		public World world;
		private List<Renderer> _renderers = new List<Renderer>();
		private List<Camera> _cameras = new List<Camera>();

		public RenderSystem(World world)
		{
			this.world = world;
		}

		public void Register(Renderer renderer)
		{
			_renderers.Add(renderer);
		}

		public void Unregister(Renderer renderer)
		{
			_renderers.Remove(renderer);
		}

		public void Register(Camera camera)
		{
			_cameras.Add(camera);
		}

		public void Unregister(Camera camera)
		{
			_cameras.Remove(camera);
		}

		private int CompareDrawOrder(Renderer a, Renderer b)
		{
			if (a.layer < b.layer)
				return -1;
			if (a.layer > b.layer)
				return 1;
			if (a.drawOrder < b.drawOrder)
				return -1;
			if (a.drawOrder > b.drawOrder)
				return 1;
			return 0;
		}

		private int CompareCameraOrder(Camera a, Camera b)
		{
			if (a.depth < b.depth)
				return -1;
			else if(a.depth > b.depth)
				return 1;
			return 0;
		}

		public void Render(RenderTarget finalTarget)
		{
			_cameras.Sort(CompareCameraOrder);

			foreach(Camera camera in _cameras)
			{
				RenderTarget target = camera.target;
				if(target == null)
				{
					target = finalTarget;
				}

				target.SetView(camera.view);

				if (camera.doClear)
				{
					target.Clear(camera.clearColor);
				}

				Render(target, RenderMode.BASE, camera.layerMask);

				if(camera.enableLighting && world.lightSystem.enabled && !Keyboard.IsKeyPressed(Keyboard.Key.L))
				{
					// Draw lights
					world.lightSystem.Draw(target, camera.layerMask);
				}
			}
		}

		public void Render(RenderTarget target, RenderMode mode, int layerMask)
		{
			List<Renderer> drawList = new List<Renderer>();
			foreach(Renderer r in _renderers)
			{
				if(((layerMask & (1<<(int)r.layer)) != 0) && r.GetMaterial(mode) != null)
				{
					drawList.Add(r);
				}
			}

			drawList.Sort(CompareDrawOrder);

			foreach(Renderer r in drawList)
			{
				r.Render(target, mode);
			}
		}

		public void OnScreenResized(Vector2u newSize)
		{
			foreach(Camera cam in _cameras)
			{
				cam.OnScreenResized(newSize);
			}
		}
	}
}

