using Framework;
using System.Collections.Generic;

namespace LD30.Components
{
    public class Flasher : Behaviour
    {
        public const float COOLDOWN_TIME = 1f;

        public int trigger = 0;
        public float coolDown = COOLDOWN_TIME;

        public void Trigger()
        {
            if(coolDown <= 0.001f)
            {
                trigger = 2;
                coolDown = COOLDOWN_TIME;
                OnFlash();
            }
        }

        public override void OnUpdate()
        {
            float dt = world.delta;
            coolDown -= dt;
            if (coolDown <= 0)
                coolDown = 0;

            base.OnUpdate();
            world.lightSystem.jamFlash = false;
            if(trigger > 0)
            {
                --trigger;
                world.lightSystem.jamFlash = true;
            }
        }

        private void OnFlash()
        {
            IEnumerable<Entity> daemons = world.taggedEntities["Daemon"];
            foreach(Entity e in daemons)
            {
                e.GetComponent<ParallelActor>().Glimpse();
            }
        }
    }
}
