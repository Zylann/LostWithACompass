using Framework;
using SFML.Graphics;
using SFML.Window;

namespace Framework
{
	public class CameraHandler : Behaviour
	{
		public Entity target;
		private Camera _camera;
        public float followSpeed = -1;
        public float shakeAmplitude;

		public override void OnUpdate()
		{
			if(_camera == null)
			{
				_camera = entity.GetComponent<Camera>();
			}

			if(_camera != null)
			{
				SpriteRenderer sprite = null;

				if(target != null)
				{
					sprite = target.sprite;
				}
				else if(world.avatar != null)
				{
					sprite = world.avatar.sprite;
				}

				if (sprite != null)
				{
					FloatRect targetBounds = sprite.GetGlobalBounds();
					Vector2f targetCenter = new Vector2f(
						targetBounds.Left + targetBounds.Width / 2f,
						targetBounds.Top + targetBounds.Height / 2f
					);

                    if(followSpeed > 0)
                    {
                        targetCenter = Mathf.Lerp(_camera.view.Center, targetCenter, followSpeed * world.delta);
                    }

                    if(shakeAmplitude > 0.001f)
                    {
                        Vector2f shakeVector = new Vector2f(
                            Random.Range(-shakeAmplitude, shakeAmplitude),
                            Random.Range(-shakeAmplitude, shakeAmplitude)
                        );
                        targetCenter += shakeVector;
                    }

                    _camera.view.Center = targetCenter;
				}
			}
		}
	}
}

