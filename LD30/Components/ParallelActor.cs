﻿using Framework;
using SFML.Window;

namespace LD30
{
    public class ParallelActor : Behaviour
    {
        private const float LIGHTABLE_DISTANCE = 6f;
        public int dimension = 0;

        protected bool IsLightedByPlayer()
        {
            if (world.avatar.body.position.DistanceTo(entity.body.position) < LIGHTABLE_DISTANCE)
            {
                float avatarLookAngle = world.avatar.sprite.rotation;
                Vector2f avatarLook = new Vector2f(
                    Mathf.Cos(avatarLookAngle * Mathf.DEG2RAD),
                    Mathf.Sin(avatarLookAngle * Mathf.DEG2RAD)
                );

                Vector2f myLook = (world.avatar.body.position - entity.body.position).Normalized();
                float dot = myLook.Dot(avatarLook);

                return dot < -0.5f;
            }
            else
                return false;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            dimension = ((LDWorld)world).dimension;
        }

        public virtual void OnDimensionChange(int dimension)
        {
            this.dimension = dimension;
        }

        public virtual void Glimpse() { }
    }
}
