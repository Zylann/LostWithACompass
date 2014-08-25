using Framework;
using SFML.Graphics;
using SFML.Window;

namespace Framework
{
	public class AudioListener : Behaviour
	{
		private Camera _camera;

		public override void OnUpdate()
		{
			Vector2f pos;
			if(GetCenter(out pos))
			{
				// Center listener on the entity (sound works in tile units)
				AudioSystem.instance.SetListenerPosition(pos);
			}
		}

		private bool GetCenter(out Vector2f pos)
		{
			if(_camera == null)
			{
				_camera = entity.GetComponent<Camera>();
			}

			if (_camera != null)
			{
				pos = _camera.view.Center / (float)world.TS;
				return true;
			}
			else if (entity.sprite != null)
			{
				FloatRect targetBounds = entity.sprite.GetGlobalBounds();
				pos = new Vector2f(
					targetBounds.Left + targetBounds.Width / 2f,
					targetBounds.Top + targetBounds.Height / 2f
				);
				pos = pos / (float)world.TS;
				return true;
			}
			else if (entity.body != null)
			{
				pos = entity.body.position + entity.body.size / 2f;
				return true;
			}
			else
			{
				pos = new Vector2f(0, 0);
				return false;
			}
		}
	}
}


