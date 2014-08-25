using SFML.Graphics;
using SFML.Window;

namespace Framework
{
	/// <summary>
	/// Basic moveable body, based on a hitbox.
	/// Only supports AABB collisions.
	/// Note: in the future, Farseer bodies would be the complete alternative.
	/// </summary>
	public class BasicBody : Component
	{
        public bool noClip = false;
        private uint collisionMask;
		private Vector2f _position;
		private FloatRect _hitbox;
		private Vector2f _velocity;
		private float _maxSpeed = 10f;

		public override void OnCreate()
		{
			entity.world.physics.Register(this);
		}

		public override void OnDestroy()
		{
			entity.world.physics.Unregister(this);
		}

        public BasicBody AddCollisionMask(int i)
        {
            collisionMask |= (uint)(1 << i);
            return this;
        }

		public BasicBody SetHitbox(FloatRect box)
		{
			_hitbox = box;
			return this;
		}

        public FloatRect hitbox
        {
            get { return _hitbox; }
        }

		public BasicBody SetMaxSpeed(float max)
		{
			_maxSpeed = max;
			return this;
		}

		public Vector2f position
		{
			get { return _position; }
			set
			{
				_position = value;
			}
		}

		public Vector2f pixelPosition
		{
			get { return _position * entity.world.TS; }
		}

		public float maxSpeed
		{
			get { return _maxSpeed; }
			set
			{
				_maxSpeed = value;
			}
		}

		public Vector2f velocity
		{
			get { return _velocity; }
			set { _velocity = value; }
		}

		public IntRect PixelBounds()
		{
			float ts = entity.world.TS;
			return new IntRect(
				(int)(ts * (_position.X + _hitbox.Left)),
				(int)(ts * (_position.Y + _hitbox.Top)),
				(int)(ts * _hitbox.Width),
				(int)(ts * _hitbox.Height)
			);
		}

		public Vector2f size
		{
			get { return new Vector2f(_hitbox.Width, _hitbox.Height); }
		}

		public bool Collides(Vector2f point)
		{
			point -= _position;
			return point.X > _hitbox.Left
				&& point.Y > _hitbox.Top
				&& point.X < _hitbox.Left + _hitbox.Width
				&& point.Y < _hitbox.Top + _hitbox.Height;
		}

		public bool Collides(FloatRect otherBox)
		{
			return _hitbox.Left + _hitbox.Width >= otherBox.Left
				&& _hitbox.Top + _hitbox.Height >= otherBox.Top
				&& otherBox.Left + otherBox.Width >= _hitbox.Left
				&& otherBox.Top + otherBox.Height >= _hitbox.Top;
		}

		public override void OnUpdate()
		{
			float speed = Mathf.Magnitude(_velocity.X, _velocity.Y);

			if(!Mathf.Approximately(speed, 0f))
			{
				float dt = entity.world.delta;

				if (speed > _maxSpeed)
				{
					_velocity /= speed;
					_velocity *= _maxSpeed;
				}

				Vector2f motion = _velocity * dt;

				Move(motion);
			}
		}

		public void Move(Vector2f motion)
		{
            if(noClip)
            {
                position += motion;
                return;
            }

			FloatRect rect = _hitbox;
			rect.Left += position.X;
			rect.Top += position.Y;

			int fractionCount = 4;
			float fraction = 1f / (float)fractionCount;
			Vector2f u = fraction * motion;

			// While motion is not zero
			for(int i = 0; i < fractionCount && !(Mathf.Approximately(motion.X, 0) && Mathf.Approximately(motion.Y, 0)); ++i)
			{
				// If motion remains on X
				if(!Mathf.Approximately(motion.X, 0))
				{
					// Advance a bit on X
					rect.Left += u.X;
					// If we collide horizontally
					if(entity.world.physics.Collides(rect))
					{
						// Horizontal collision event

						// Go back
						rect.Left -= u.X;
						// Stop motion on X
						motion.X = 0;
						u.X = 0;
					}
					else
					{
						// Remove the part of motion we just moved
						motion.X -= u.X;
						// If there is no more to advance on X
						if(Mathf.Approximately(motion.X, 0))
						{
							// Finished to advance on X
							motion.X = 0;
							u.X = 0;
						}
					}
				}

				// Same on Y
				if(!Mathf.Approximately(motion.Y, 0))
				{
					rect.Top += u.Y;
					if (entity.world.physics.Collides(rect))
					{
						// Vertical collision

						rect.Top -= u.Y;
						motion.Y = 0;
						u.Y = 0;
					}
					else
					{
						motion.Y -= u.Y;
						if(Mathf.Approximately(motion.Y, 0))
						{
							motion.Y = 0;
							u.Y = 0;
						}
					}
				}
			}

			_position.X = rect.Left - _hitbox.Left;
			_position.Y = rect.Top - _hitbox.Top;
		}
	}
}

