using Framework;
using LD30.Components;
using SFML.Graphics;
using SFML.Window;

namespace LD30.Actors
{
    public class Portal : ParallelActor
    {
        public const float GLIMPSE_TIME = 10f;

        private DaemonRenderer _renderer;
        private float _glimpseTime;
        private bool _trigger;
        public int targetDimension = 1;

        public override void OnCreate()
        {
            entity.AddTag("Daemon");
            entity.AddTag("Portal");

            base.OnCreate();

            _renderer = entity.AddComponent<DaemonRenderer>();
            _renderer.SetTexture("portal", RenderMode.BASE).SetLayer(ViewLayers.OBJECTS);
            _renderer.SetTexture("portal_lightmap", RenderMode.LIGHT_MAP);
            entity.AddComponent<BasicBody>().SetHitbox(new SFML.Graphics.FloatRect(-0.5f,-0.5f,1.0f,1.0f));

            entity.AddComponent<AudioEmitter>();
            entity.audio.Play("portal_loop", 1f, 1f, true);

            _renderer.color = new Color(255, 64, 64, 0);
        }

        public override void Glimpse()
        {
            Vector2f avatarPos = world.avatar.body.position;
            if(entity.body.position.DistanceTo(avatarPos) < world.avatar.GetComponent<Light>().radius)
            {
                _glimpseTime = GLIMPSE_TIME;
            }
            else
            {
                _glimpseTime = 1f;
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            _renderer.rotation += 180f*world.delta;

            _glimpseTime -= world.delta;
            if(_glimpseTime < 0)
            {
                _glimpseTime = 0;
            }

            Vector2f avatarPos = world.avatar.body.position;
            Vector2f pos = entity.body.position;
            float distanceToAvatar = pos.DistanceTo(avatarPos);
            if (distanceToAvatar < entity.body.hitbox.Width/2)
            {

                if (!_trigger)
                {
                    OnAvatarReach();
                    _trigger = true;
                }
            }
            else
                _trigger = false;


            _renderer.color = new Color(255, 255, 255, (byte)(255f * System.Math.Max(Mathf.Clamp01(_glimpseTime), Mathf.Clamp01(1f-distanceToAvatar/4f))));

        }

        public override void OnDimensionChange(int newDim)
        {
            if (newDim == 1)
                targetDimension = 0;
            else
                targetDimension = 1;
        }

        public void Reposition()
        {
            Vector2i avatarPos = new Vector2i((int)world.avatar.body.position.X, (int)world.avatar.body.position.Y);
            Vector2i newPos = ((LDWorld)world).FindSpawnableZone(p => p.DistanceTo(avatarPos) > 15);
            entity.body.position = new Vector2f(newPos.X, newPos.Y);
        }

        private void OnAvatarReach()
        {
            var w = (LDWorld)world;
            w.SetDimension(targetDimension);
            world.avatar.GetComponent<Flasher>().Trigger();
            Reposition();
            entity.audio.Play("warp");
        }
    }
}







