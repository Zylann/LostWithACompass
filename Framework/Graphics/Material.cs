using SFML.Graphics;

namespace Framework
{
	public class Material
	{
		public Shader shader;
		public Texture mainTexture;
		public BlendMode blendMode = BlendMode.Alpha;

		public Material()
		{
		}

		public Material(string textureName, Texture texture, BlendMode blendMode=BlendMode.Alpha, string shaderName="")
		{
			mainTexture = string.IsNullOrEmpty(textureName) ? null : Assets.textures[textureName];
			shader = string.IsNullOrEmpty(shaderName) ? null : Assets.shaders[shaderName];
			this.blendMode = blendMode;
		}

		public Material(Texture texture, BlendMode blendMode=BlendMode.Alpha, Shader shader=null)
		{
			mainTexture = texture;
			this.shader = shader;
			this.blendMode = blendMode;
		}
	}
}


