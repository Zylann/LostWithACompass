using Framework;

namespace LD30.Actors
{
    public class MainCamera : Entity
    {
        private CameraHandler _handler;

        public override void OnCreate()
        {
            base.OnCreate();
            Entity entity = this;
            entity.AddComponent<Camera>().AddLayersUnder(ViewLayers.FOREGROUND).RemoveLayer(ViewLayers.BACKGROUND).RemoveLayer(ViewLayers.DAEMON_OBJECTS);
            entity.AddComponent<AudioListener>();
            _handler = entity.AddComponent<CameraHandler>();
            entity.camera.enableLighting = true;
            world.mainCamera = entity.camera;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            _handler.shakeAmplitude = 2.5f*(1f-Mathf.Clamp01(world.avatar.GetComponent<Avatar>().enemyDistance / 3f));
        }
    }
}


