using Framework;
using LD30.Components;
using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;

namespace LD30.Actors
{
    public class Avatar : Behaviour
    {
        public const float HP_LOSS_SPEED = 0.5f;
        public const float HP_REGEN_SPEED = 0.04f;
		public const float HP_BADWORLD_LOSS_SPEED = 0.03f;

        public Light light;
        private Flasher flasher;
        public float hp = 0.75f;
        private float _lookRotation;

        public float enemyDistance;
        public bool hurtState = false;

        public override void OnCreate()
        {
            base.OnCreate();
            entity.name = "bob";
            entity.AddComponent<SpriteRenderer>().SetTexture(Assets.textures["avatar"]).SetLayer(ViewLayers.OBJECTS);
            entity.AddComponent<BasicBody>().SetHitbox(new FloatRect(-0.3f, -0.3f, 0.6f, 0.6f)).SetMaxSpeed(20f);
            entity.AddComponent<FootstepsSound>().SetStep(1.5f).SetVars("step", 1, 8).SetVolume(0.25f);
            entity.AddComponent<AudioEmitter>().Spatialize();
            entity.AddComponent<SpriteOrderFromY>();
            entity.AddComponent<AudioListener>();
            world.avatar = this.entity;

            light = entity.AddComponent<Light>().SetColor(new Color(255, 255, 255)).AddLayer(ViewLayers.TERRAIN).SetRadius(250f);
            light.SetCookie("flashcone3");
            flasher = entity.AddComponent<Flasher>();
           
            entity.sprite.textureRect = new IntRect(0, 0, 32, 32);
            entity.sprite.CenterOrigin();

            entity.AddComponent<Gun>();
        }

        private Vector2f Control()
        {
            Vector2f motor = new Vector2f(0, 0);

            if (Keyboard.IsKeyPressed(Keyboard.Key.Q)
                || Keyboard.IsKeyPressed(Keyboard.Key.Left)
                || Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                motor.X -= 1f;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.D)
                || Keyboard.IsKeyPressed(Keyboard.Key.Right))
            {
                motor.X += 1f;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.S)
                || Keyboard.IsKeyPressed(Keyboard.Key.Down))
            {
                motor.Y += 1f;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Z)
                || Keyboard.IsKeyPressed(Keyboard.Key.Up)
                || Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                motor.Y -= 1f;
            }

            //if(Keyboard.IsKeyPressed(Keyboard.Key.Space))
            //{
            //    flasher.Trigger();
            //}

            return motor;
        }

        public float lookRotation
        {
            get { return _lookRotation; }
        }

        public override void OnUpdate()
        {
            // Not really accurate but fuck it
            Vector2f mousePos = Application.instance.cursorPosition;
            mousePos.X -= Application.instance.ScreenSize.X / 2;
            mousePos.Y -= Application.instance.ScreenSize.Y / 2;

            Vector2f lookVector = mousePos;
            float lookAngle = lookVector.Angle() * Mathf.RAD2DEG;
            _lookRotation = lookAngle;
            entity.sprite.rotation = lookAngle;
            light.SetCookieRotation(lookAngle);

            light.intensity = Random.Range(0.95f, 1f);

            float gbk = 1f-Mathf.Pow3(Mathf.Abs(hp-1));
            byte gb = (byte)(gbk * 255f);
            light.color = new Color(255, gb, gb);
            DebugOverlay.instance.Line("HP", hp);

            Vector2f motor = Control();

            float mag = Mathf.Magnitude(motor.X, motor.Y);
            if (!Mathf.Approximately(mag, 0f))
            {
                motor /= mag;

                float speed = 5;
                entity.body.velocity = motor * speed;

                // Walk anim
                //entity.sprite.textureRect = new IntRect(Game.TS * (1 + (world.timeMs / 50) % 4), Game.TS * _direction, 32, 32);
            }
            else
            {
                entity.body.velocity = new Vector2f();

                // Idle
                //entity.sprite.textureRect = new IntRect(0, Game.TS * _direction, 32, 32);
            }

            // Enemies hurting

            IEnumerable<Entity> enemies = world.taggedEntities["Enemy"];
            enemyDistance = 9999f;
            foreach(Entity e in enemies)
            {
                float d = e.body.position.DistanceTo(this.entity.body.position);
                if (d < enemyDistance)
                    enemyDistance = d;
            }

            hurtState = enemyDistance < entity.body.hitbox.Width;

            if (hurtState)
            {
                hp -= world.delta * HP_LOSS_SPEED;
                if (hp <= 0)
                    Kill();
            }
            else
            {
                if(((LDWorld)world).dimension == 0)
                {
                    hp += world.delta * HP_REGEN_SPEED;
                    if (hp >= 1)
                        hp = 1;
                }
                else
                {
					hp -= world.delta * HP_BADWORLD_LOSS_SPEED;
                    if (hp <= 0)
                        Kill();
                }
            }

        }

        public bool IsDead()
        {
            return hp <= 0f;
        }

        public void Kill()
        {

        }
    }
}


