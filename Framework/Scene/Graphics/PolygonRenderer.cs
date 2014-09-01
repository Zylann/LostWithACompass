using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Scene.Graphics
{
    public class PolygonRenderer : Renderer
    {
        private VertexArray _mesh = new VertexArray();
        private Vector2f _position;
        private float _rotation;
        private Vector2f _scale = new Vector2f(1f, 1f);

        public Vector2f position
        {
            get { return _position; }
            set { _position = value; }
        }

        public float rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public Vector2f scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public void MakeRectangle(FloatRect rect, FloatRect uv)
        {
            _mesh.Clear();

            Vertex v = new Vertex();
            Color color = Color.White;

            v.Color = color;
            v.Position = new Vector2f(rect.Left, rect.Top);
            v.TexCoords = new Vector2f(uv.Left, uv.Top);
            _mesh.Append(v);

            v.Position.X += rect.Width;
            v.TexCoords.X += uv.Width;
            _mesh.Append(v);

            v.Position.Y += rect.Height;
            v.TexCoords.Y += uv.Height;
            _mesh.Append(v);

            v.Position.X = rect.Left;
            v.TexCoords.X = uv.Left;
            _mesh.Append(v);

            _mesh.PrimitiveType = PrimitiveType.Quads;
        }

        public override void Render(RenderTarget rt, RenderMode mode)
        {
            Material mat = GetMaterial(mode);
            if (mat != null)
            {
                RenderStates states = new RenderStates(mat.shader);
                states.BlendMode = mat.blendMode;
                states.Transform.Rotate(_rotation);
                states.Transform.Scale(_scale);
                states.Transform.Translate(_position);
                states.Texture = mat.mainTexture;
                rt.Draw(_mesh, states);
            }
        }
    }
}


