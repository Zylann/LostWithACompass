using Framework;
using SFML.Window;

namespace LD30.Components
{
    public class Gun : Behaviour
    {
        public const float COOLDOWN_VALUE = 0.3f;

        private float _cooldown;
        private bool _canShoot = true;

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if(_cooldown > 0)
            {
                _cooldown -= world.delta;
                if(_cooldown <= 0)
                {
                    _cooldown = 0;
                    _canShoot = true;
                }
            }
            
            if(Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                if(_canShoot)
                {
                    Shoot();
                    _cooldown = COOLDOWN_VALUE;
                    _canShoot = false;
                }
            }
        }

        private void Shoot()
        {
            Bullet b = world.SpawnEntity().AddComponent<Bullet>();
            b.rotation = entity.sprite.rotation;
            b.position = entity.body.position;
			entity.audio.Play("shoot1", 0.3f);
        }
    }
}
