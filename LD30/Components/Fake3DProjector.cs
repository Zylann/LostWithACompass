using Framework;
using SFML.Graphics;
using SFML.Window;

namespace LD30
{
    /// <summary>
    /// Uses the filtermap from the shader-based light system to project fake 3D walls from the center of the screen
    /// </summary>
    public class Fake3DProjector : Behaviour
    {
        /// <summary>
        /// Camera from which we see the game world
        /// </summary>
        public Camera camera;

        private const uint RT_SIZE = 256;
        private RenderTexture _rt;
        private RenderTexture _filterMap;
        private Shader _shader;
        private RectangleShape _fillRect;

        public RenderTexture renderTexture { get { return _rt; } }

        public override void OnCreate()
        {
            base.OnCreate();
            _shader = Assets.shaders["fake3d"];
            _filterMap = new RenderTexture(RT_SIZE, RT_SIZE);
            _rt = new RenderTexture(RT_SIZE, RT_SIZE);
            _rt.SetView(new View(new FloatRect(0, 0, _filterMap.Size.X, _filterMap.Size.Y)));
            _fillRect = new RectangleShape(new Vector2f(_rt.Size.X, _rt.Size.Y));
            _fillRect.FillColor = new Color(255, 255, 255, 255);
        }

        public void Render()
        {
            // Render filtermap under the light
            _filterMap.SetView(camera.view);
            _filterMap.Clear(Color.White);
            world.graphics.Render(_filterMap, RenderMode.LIGHT_FILTER, camera.layerMask);
            _filterMap.Display();

            // Configure light shader
            _shader.SetParameter("lightPos", new Vector2f(_filterMap.Size.X / 2, _filterMap.Size.Y / 2));
            _shader.SetParameter("filterMap", _filterMap.Texture);
            _shader.SetParameter("lightColor", Color.White);
            _shader.SetParameter("fragOffset", new Vector2f(0,0));

            // Render the projection
            RenderStates renderStates = new RenderStates(_shader);
            renderStates.BlendMode = BlendMode.None;
            _rt.Draw(_fillRect, renderStates);
        }
    }
}

