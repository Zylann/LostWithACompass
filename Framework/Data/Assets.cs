using System.Collections.Generic;
using SFML.Graphics;
using SFML.Audio;
using System.IO;

namespace Framework
{
	/// <summary>
	/// Assets currently loaded in memory
	/// </summary>
	public class Assets
	{
		public static string root = "Assets";

		public static Dictionary<string,Texture> textures = new Dictionary<string,Texture>();
		public static Dictionary<string, SoundBuffer> soundBuffers = new Dictionary<string, SoundBuffer>();
		public static Dictionary<string, string> streamedSounds = new Dictionary<string, string>();
        public static Dictionary<string, Font> fonts = new Dictionary<string, Font>();
		public static Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();

		public static void Load()
		{
			Log.Info("Loading assets...");

			foreach (string filePath in Directory.EnumerateFiles(Path.Combine(".",root), "*.*", SearchOption.AllDirectories))
			{
				// Do not load files starting with an underscore
				string fileName = Path.GetFileNameWithoutExtension(filePath);
				if (fileName.StartsWith("_"))
				{
					continue;
				}

				string ext = Path.GetExtension(filePath);
				if (ext == ".png")
				{
					LoadTexture(filePath);
				}
				else if(ext == ".ogg" || ext == ".wav")
				{
					// TODO smarter sound stream detection
                    if (filePath.Contains("streamed") || filePath.Contains("Streamed"))
					{
						LoadStreamedSound(filePath);
					}
					else
					{
						LoadSoundBuffer(filePath);
					}
				}
                else if(ext == ".ttf")
                {
                    LoadFont(filePath);
                }
				else if(ext == ".vs")
				{
					LoadShader(filePath);
				}
			}
		}

        private static void LoadFont(string path)
        {
            Log.Info("Loading " + path + "...");
            Font font = new Font(path);
            fonts.Add(GetNameFromPath(path), font);
        }

		private static void LoadStreamedSound(string path)
		{
			Log.Info("Loading streamed sound " + path + "...");

			if (!File.Exists(path))
			{
				throw new FileNotFoundException("Streamed sound not found", path);
			}
			else
			{
				streamedSounds.Add(GetNameFromPath(path), path);
			}
		}

		private static string GetNameFromPath(string path)
		{
			return System.IO.Path.GetFileNameWithoutExtension(path);
		}

		public static void LoadTexture(string path)
		{
			Log.Info("Loading " + path + "...");
			Texture texture = new Texture(path);
			texture.Smooth = false;
			textures.Add(GetNameFromPath(path), texture);
		}

		public static void LoadSoundBuffer(string path)
		{
			Log.Info("Loading " + path + "...");
			SoundBuffer sound = new SoundBuffer(path);
			soundBuffers.Add(GetNameFromPath(path), sound);
		}

		public static void LoadShader(string vsPath)
		{
			Log.Info("Loading " + vsPath + "...");
			string fsPath = vsPath.Substring(0, vsPath.LastIndexOf(".")) + ".fs";
			Shader shader = new Shader(vsPath, fsPath);
			shaders.Add(GetNameFromPath(vsPath), shader);
		}
	}
}

