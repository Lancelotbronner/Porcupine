using System.Diagnostics;
using System.Reflection;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Porcupine.Rendering;

public static class Resources {

	#region Search Paths Management

	public static readonly Assembly Assembly = typeof(Resources).Assembly;

	public static Stream Find(string filename, string path) {
		if (Assembly.GetManifestResourceStream($"{path}.{filename}") is not Stream stream) {
			Debug.WriteLine($"Could not locate {filename} within path");
			throw new FileNotFoundException(path);
		}
		return stream;
	}

	#endregion

	#region Terrain Atlas Management

	public static TextureHandle TerrainAtlas { get; } = CreateTerrainAtlas();

	private static TextureHandle CreateTerrainAtlas() {
		using Stream stream = Find("tiles.png", "Porcupine.Textures");
		using Image<Rgba32> image = Image.Load<Rgba32>(stream);

		Span<byte> pixels = stackalloc byte[image.Width * image.Height * 4];
		image.CopyPixelDataTo(pixels);

		TextureHandle texture = GL.GenTexture();
		GL.BindTexture(TextureTarget.Texture2d, texture);
		GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (ReadOnlySpan<byte>)pixels);

		GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
		GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
		GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
		GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

		return texture;
	}

	#endregion

}
