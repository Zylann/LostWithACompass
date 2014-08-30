using Framework;
using Framework.Pcg;
using LD30.Components;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

namespace LD30.Actors
{
    public class Daemon : ParallelActor
    {
        private const float APPEAR_BY_LIGHT_TIME = 0.2f;

        private float noiseOffset;
        private float _alpha = 0f;
        private DaemonRenderer _renderer;

        public int hp = 5;

        private float lightedTime;
        private float alphaByLighting;

        private float timeBeforeSound;
        private FloatRange soundTime = new FloatRange(16f, 32f);

		private float speed = 1f;

		/// <summary>
		/// How many frames left before the daemon stops flickering
		/// YES
		/// DOC COMMENTS
		/// WHEN THERE IS ONLY 4 HOURS LEFT #LD48
		/// </summary>
		private int _flicker;

        public override void OnCreate()
        {
            entity.AddTag("Daemon");
            entity.AddTag("Enemy");

			entity.name = "daemon";

            base.OnCreate();
            _renderer = entity.AddComponent<DaemonRenderer>();
            UpdateAppearance(((LDWorld)world).dimension);

            entity.AddComponent<AudioEmitter>().Spatialize();
			entity.AddComponent<SpriteOrderFromY>();

            entity.AddComponent<BasicBody>().SetHitbox(new FloatRect(-0.5f,-0.5f,1f,1f));
            entity.body.noClip = true;
            //entity.AddComponent<Light>().SetColor(new Color(255, 0, 255)).SetRadius(200).AddLayer(ViewLayers.TERRAIN);

            timeBeforeSound = soundTime.Random();

            noiseOffset = Random.Range(0f, 1000f);
        }

        private void UpdateAppearance(int dim)
        {
            if(dim == 0)
            {
                _renderer.SetTexture("daemon1").SetLayer(ViewLayers.OBJECTS);
                _renderer.SetTexture("daemon1_lightmap", RenderMode.LIGHT_MAP);
                _renderer.enableTrail = true;
            }
            else
            {
                _renderer.SetTexture("daemon1p").SetLayer(ViewLayers.OBJECTS);
                _renderer.SetMaterial(null, RenderMode.LIGHT_MAP);
                _renderer.enableTrail = false;
            }
        }

        public override void OnDimensionChange(int dimension)
        {
            base.OnDimensionChange(dimension);
            UpdateAppearance(dimension);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            // AI ----------------------

            Vector2f avatarPos = world.avatar.body.position;
            Vector2f pos = entity.body.position;

            float distanceToTarget = avatarPos.DistanceTo(pos);
            float targetBias = Mathf.Clamp(distanceToTarget / 2f, 2f, 10f);

            float time = ((float)world.timeMs) / 1000f;
            float roff = time + noiseOffset;

            Vector2f bias = targetBias * new Vector2f(
                2f * Noise.GetPerlin(roff, 10, 6568, 2, 0.5f, 1f) - 1f,
                2f * Noise.GetPerlin(roff, 100, 5468, 2, 0.5f, 1f) - 1f
            );

            Vector2f dir = (avatarPos + bias) - pos;

            //float v = 3f + 3f*Noise.GetPerlin(roff, 0, 123456, 2, 0.5f, 1f);

            float speed = 1.5f;
            if(dimension == 1 && distanceToTarget < 8f)
            {
                speed = 3f;
            }

            entity.body.velocity = dir.Normalized() * speed;

            // Lightening and rendering -----------------------------

            if(dimension == 0)
            {
                if (IsLightedByPlayer())
                {
                    lightedTime += world.delta;
                }
                else
                {
                    lightedTime = 0;
                }
                alphaByLighting = Mathf.Clamp01(System.Math.Max(lightedTime - APPEAR_BY_LIGHT_TIME, alphaByLighting - world.delta));

                float dt = world.delta;
                if (_alpha > 0)
                {
                    _alpha -= dt;
                    if (_alpha <= 0)
                        _alpha = 0;
                }
                float contactAlpha = 1f - Mathf.Clamp01(distanceToTarget);
                float instantAlpha = System.Math.Max(contactAlpha, _alpha);
                instantAlpha = System.Math.Max(instantAlpha, alphaByLighting);
                _renderer.color = new Color(255, 255, 255, (byte)(instantAlpha * 255f));
                _renderer.textureRect = new IntRect(
                    0,
                    0,
                    64,
                    64
                );
            }
            else
            {
                _renderer.color = _flicker<=0 ? Color.White : new Color(128,128,128,128);
                _renderer.textureRect = new IntRect(
                    64 * ((world.timeMs/64) % 4),
                    0,
                    64,
                    64
                );
            }

			--_flicker;

            // Sound -----------------------------------

            timeBeforeSound -= world.delta;
            if(timeBeforeSound <= 0)
            {
                timeBeforeSound = soundTime.Random();
                entity.audio.Play("enemy" + Random.Range(1, 7 + 1), 1f, Random.Range(0.9f,1.1f), false, AudioCategories.ENEMIES);
            }
        }

        public void Hurt(int damage)
        {
            Log.Debug("Enemy hurt");
            hp -= damage;
			_flicker = 2;
			if (hp <= 0)
            {
                Kill();
            }
        }

        public void Kill()
        {
			Game.score += 1;
			PlayDeathSound();
            entity.DestroyLater();

			SpawnDaemon();
			SpawnDaemon();
        }

		private void SpawnDaemon()
		{
			Daemon daemon = world.SpawnEntity().AddComponent<Daemon>();
			daemon.Reposition();
		}

		private void Reposition()
		{
			Vector2i avatarPos = new Vector2i((int)world.avatar.body.position.X, (int)world.avatar.body.position.Y);
			Vector2i newPos = ((LDWorld)world).FindSpawnableZone(p => p.DistanceTo(avatarPos) > 15);
			entity.body.position = new Vector2f(newPos.X, newPos.Y);
		}

		private void PlayDeathSound()
		{
			Vector2f pos = entity.body.position;
			AudioSystem.instance.Play(
				Assets.soundBuffers["enemy_death" + Random.Range(1, 4 + 1)],
				new Vector3f(pos.X, pos.Y, 0f), 1f, Random.Range(0.8f, 1.2f)
			);
		}

        public override void Glimpse()
        {
            _alpha = 1f;
        }
    }
}


