using Framework;
using Framework.Scene.Graphics;
using SFML.Graphics;
using SFML.Window;

namespace LD30.Components
{
    public class DaemonRenderer : SpriteBatchRenderer
    {
        struct PosItem
        {
            public Vector2f pos;
            public float rotation;
            public PosItem(Vector2f pos0, float rotation0) { pos = pos0; rotation = rotation0; }
        }

        private PosItem[] _posBuffer = new PosItem[32];
        private int _offset;
        public Color color;
        public float rotation = 0;
        public IntRect textureRect = new IntRect(0,0,64,64);
        public bool enableTrail = true;

        protected override void OnDrawSprites(SpriteBatch batch)
        {
            Vector2f center = entity.body.pixelPosition;
            _posBuffer[_offset % _posBuffer.Length] = new PosItem(center, rotation);

            if (color.A == 0)
                return;
            
            if(!enableTrail)
            {
                batch.DrawCentered(center, textureRect, color, new Vector2f(1, 1), rotation);
                return;
            }

            Color col = color;

            for(int i = 0; i < _posBuffer.Length; ++i)
            {
                int j = (i + _offset) % _posBuffer.Length;
                PosItem p = _posBuffer[j];
                col.A = (byte)((float)(color.A) * (float)i / (float)_posBuffer.Length);
                batch.DrawCentered(p.pos, textureRect, col, new Vector2f(1, 1), p.rotation);
            }

            ++_offset;
        }

    }
}


