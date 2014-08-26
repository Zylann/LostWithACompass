using Framework;
using SFML.Graphics;
using SFML.Window;

namespace LD30
{
    public class Compass
    {
        public Sprite bg;
        public Sprite arrow;
        private float _angle;

        public void Init()
        {
            bg = new Sprite(Assets.textures["compass"]);
            bg.Origin = new Vector2f(16, 16);

            arrow = new Sprite(Assets.textures["compass_arrow"]);
            arrow.Origin = new Vector2f(16, 16);

            UpdatePosition();
        }

        public void Render(RenderTarget rt)
        {
            rt.Draw(bg);
            rt.Draw(arrow);
        }

        public void Update(float angle)
        {
            _angle = angle;
            arrow.Rotation = _angle;
        }

        public void UpdatePosition()
        {
            Vector2u ss = Application.instance.ScreenSize;
            bg.Position = new Vector2f(16 + 4, ss.Y - 16 - 4);
            arrow.Position = bg.Position;
        }
    }
}
