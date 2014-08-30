using Framework;
using LD30.Actors;
using SFML.Audio;
using SFML.Window;
using System.Collections.Generic;

namespace LD30.Components
{
    public class Bullet : Behaviour
    {
        public float rotation;
        public float speed = 10f;
        public Vector2f position;

        public float lifeTime;
		private bool _traversedOnce;

        public override void OnCreate()
        {
            base.OnCreate();
			entity.name = "bullet";
            entity.AddComponent<SpriteRenderer>().SetTexture("bullet").SetLayer(ViewLayers.OBJECTS);
			entity.AddComponent<AudioEmitter>().Spatialize();
        }

		private void PlayHitSound()
		{
			string soundName = "hit" + Random.Range(1, 3 + 1);
			AudioSystem.instance.Play(Assets.soundBuffers[soundName], new Vector3f(position.X, position.Y, 0f), 0.2f, Random.Range(0.8f, 1.2f), false, false);
		}

		private void PlayTraverseSound()
		{
			entity.audio.Play("traverse", 1f, Random.Range(0.8f, 1.2f));
		}
        
        public override void OnUpdate()
        {
			Vector2f lastPosition = position;

            base.OnUpdate();
            position += new Vector2f(
                speed * world.delta * Mathf.Cos(rotation * Mathf.DEG2RAD),
                speed * world.delta * Mathf.Sin(rotation * Mathf.DEG2RAD)
            );
            entity.sprite.position = position * Game.TS;

			LDWorld ldworld = (LDWorld)world;

			if(ldworld.map.mapCollider.Collides(position))
			{
				entity.DestroyLater();
				PlayHitSound();
			}

            IEnumerable<Entity> enemies = world.GetTaggedEntities("Enemy");
            foreach(Entity e in enemies)
            {
                Daemon daemon = e.GetComponent<Daemon>();
                if (e.body.Collides(position))
                {
					if(ldworld.dimension == 1)
					{
						//world.debugLines.DrawLine(position, lastPosition);
						daemon.Hurt(1);
						entity.DestroyLater();
						PlayHitSound();
					}
					else if(!_traversedOnce)
					{
						PlayTraverseSound();
						_traversedOnce = true;
					}
				}
            }

            lifeTime += world.delta;
            if(lifeTime > 2f)
            {
                entity.DestroyLater();
            }
        }
    }
}
