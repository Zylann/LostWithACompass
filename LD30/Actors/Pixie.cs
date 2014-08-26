using Framework;
using LD30.Components;
using SFML.Graphics;
using SFML.Window;

namespace LD30.Actors
{
    public class Pixie : ParallelActor
    {
        private const float GLIMPSE_TIME = 5f;

        private float _alpha = 0f;
        private DaemonRenderer _renderer;
        private Vector2f referencePosition;
        private float timeBeforeNextGlimpse;
        private float glimpseTime;
        private float _rand;
        private float _wanderRadius;

        public override void OnCreate()
        {
            entity.AddTag("Daemon");

			entity.name = "pixie";

            base.OnCreate();
            _renderer = entity.AddComponent<DaemonRenderer>();
            _renderer.SetTexture("pixie").SetLayer(ViewLayers.FOREGROUND);
            _renderer.SetTexture("pixie_lightmap", RenderMode.LIGHT_MAP);

            entity.AddComponent<BasicBody>().SetHitbox(new FloatRect(16,16,32,32));
            entity.body.noClip = true;
            //entity.AddComponent<Light>().SetColor(new Color(255, 0, 255)).SetRadius(200).AddLayer(ViewLayers.TERRAIN);

            LDWorld w = (LDWorld)world;
            referencePosition = new Vector2f(
                Random.Range(0, w.map.cells.sizeX),
                Random.Range(0, w.map.cells.sizeY)
            );

            _rand = Random.Range(0f, 1000f);
            _wanderRadius = Random.Range(6f, 10f);
        }

        public override void Glimpse()
        {
            timeBeforeNextGlimpse = Random.Range(10f, 20f);
            glimpseTime = GLIMPSE_TIME;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            float t = (((float)world.timeMs) / 1000f) * 0.1f + _rand;
            float r = _wanderRadius;
            Vector2f wanderPos = new Vector2f(
                r*Mathf.Cos(t),
                r*Mathf.Sin(2f*t)
            );
            entity.body.position = referencePosition + wanderPos;

            timeBeforeNextGlimpse -= world.delta;
            if(timeBeforeNextGlimpse <= 0)
            {
                Glimpse();
            }

            if(glimpseTime > 0)
            {
                glimpseTime -= world.delta;
                if (glimpseTime < 0)
                    glimpseTime = 0;

                float a = glimpseTime / GLIMPSE_TIME;
                a = 1f - Mathf.Sq(2f*a - 1f);
                _renderer.color = new Color(255, 255, 255, (byte)(255f * a));
            }

            int frame = (world.timeMs / 32) % 3;
            _renderer.textureRect = new IntRect(frame*32, 0, 32, 32);
        }
    }
}


