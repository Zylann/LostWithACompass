using Framework;
using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD30
{
    public class HealthBar
    {
        public Sprite head;
        public Sprite bar;
        public float maxWidth = 10f;
        private float _value;
        private float _previousValue;

        public void Init()
        {
            Vector2f position = new Vector2f(8,8);

            head = new Sprite(Assets.textures["player"]);
            head.Origin = new Vector2f(0, 0);
            head.Position = position;
            head.Color = new Color(255, 255, 255, 160);
            
            bar = new Sprite(Assets.textures["health"]);
            bar.Origin = new Vector2f(0,0);
            bar.Position = position + new Vector2f(28, 10);
        }

        public void Update(float healthValue)
        {
            _previousValue = _value;
            _value = healthValue;
            bar.Scale = new Vector2f(_value * maxWidth, 1f);
        }

        public void Render(RenderTarget rt)
        {
            byte a = 160;

			bar.Color = _value < _previousValue ?
				new Color(
					(byte)Random.Range(50, 255),
					(byte)Random.Range(100, 155), 
					(byte)Random.Range(0, 50), a
				) 
			:
				new Color(32, 200, 0, a);

            head.TextureRect = new IntRect(0, 0, 32, 32);

            if (_value < 0.6f)
            {
                if(_value < 0.3f)
                {
                    bar.Color = new Color(200, 0, 0, a);

                    if(_value <= 0f)
                        head.TextureRect = new IntRect(96, 0, 32, 32);
                    else
                        head.TextureRect = new IntRect(64, 0, 32, 32);
                }
                else
                {
                    bar.Color = new Color(200,150,0, a);
                    head.TextureRect = new IntRect(32, 0, 32, 32);
                }
            }

            rt.Draw(bar);
            rt.Draw(head);
        }
    }
}
