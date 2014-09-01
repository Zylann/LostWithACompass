using Framework;
using Framework.Scene.Graphics;
using SFML.Graphics;
using SFML.Window;

namespace LD30
{
    public class MainCamera : Entity
    {
        private CameraHandler _handler;
        private Fake3DProjector _fake3d;
        private PolygonRenderer _fake3dOverlay;

        public override void OnCreate()
        {
            base.OnCreate();

            Entity entity = this;

            Camera camera = entity.AddComponent<Camera>().AddLayersUnder(ViewLayers.FOREGROUND).RemoveLayer(ViewLayers.BACKGROUND).RemoveLayer(ViewLayers.DAEMON_OBJECTS);
            entity.camera.enableLighting = true;

            entity.AddComponent<AudioListener>();
            
            _fake3d = entity.AddComponent<Fake3DProjector>();
            _fake3d.camera = camera;
            
            _handler = entity.AddComponent<CameraHandler>();
            _handler.followSpeed = 20;

            _fake3dOverlay = entity.AddComponent<PolygonRenderer>();
            _fake3dOverlay.SetMaterial(new Material(_fake3d.renderTexture.Texture, BlendMode.Multiply)).SetLayer(ViewLayers.FOREGROUND);
            //_fake3dSprite.CenterOrigin();

            world.mainCamera = entity.camera;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            _handler.shakeAmplitude = 2.5f*(1f-Mathf.Clamp01(world.avatar.GetComponent<Avatar>().enemyDistance / 3f));
            
            _fake3d.Render();

            Vector2f s = camera.view.Size;

            _fake3dOverlay.MakeRectangle(
                new FloatRect(0, 0, s.X, s.Y),
                new FloatRect(0, _fake3d.renderTexture.Size.Y, _fake3d.renderTexture.Size.X, -_fake3d.renderTexture.Size.Y)
            );

            _fake3dOverlay.position = camera.view.Center - s / 2f;
        }
    }
}


