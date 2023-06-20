using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Porcupine.Registries;
using Porcupine.Shaders;
using Porcupine.Worlds;

namespace Porcupine.Rendering;

internal readonly struct GeometryChunkRenderer : IChunkRenderer {

	internal GeometryChunkRenderer(GeometryChunkRendererFactory factory, Chunk chunk) {
		_factory = factory;
		_x = chunk.X;
		_y = chunk.Y;

		// Prepare the vertex buffer object
		GL.GenBuffer(out _vbo);
		UpdateVertexBuffer(chunk);

		// Prepare the vertex array object
		GL.GenVertexArray(out _vao);
		GL.BindVertexArray(_vao);
		GL.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

		// Add the tile id attribute
		GL.EnableVertexAttribArray(0);
		GL.VertexAttribIPointer(0, 1, VertexAttribIType.UnsignedInt, sizeof(uint), 0);

		// Register chunk callbacks
		chunk.TerrainChanged += UpdateVertexBuffer;
	}

	private readonly GeometryChunkRendererFactory _factory;
	private readonly int _x;
	private readonly int _y;
	private readonly BufferHandle _vbo;
	private readonly VertexArrayHandle _vao;

	public readonly void Update(in Camera camera, in FrameEventArgs frame) {
		_factory.SetChunkPosition(_x, _y);
		GL.BindVertexArray(_vao);
		GL.DrawArrays(PrimitiveType.Points, 0, Chunk.SIZE);
	}

	public readonly void UpdateVertexBuffer(Chunk chunk) {
		GL.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
		GL.BufferData(BufferTargetARB.ArrayBuffer, chunk.Tiles, BufferUsageARB.StaticDraw);
	}

	public readonly void Dispose() {
		GL.DeleteVertexArray(_vao);
		GL.DeleteBuffer(_vbo);
	}

}

internal sealed class GeometryChunkRendererFactory : IChunkRendererFactory {

	public GeometryChunkRendererFactory() {
		// Compile and prepare shaders
		const string shader = "TilemapRenderer";
		ShaderCompiler compiler = new();
		compiler.CompileVertex(shader);
		compiler.CompileFragment(shader);
		compiler.CompileGeometry(shader);
		compiler.Finalize(out ProgramHandle program);
		Shader = program;

		// Prepare the related buffers
		TexturesUniform = GL.GenBuffer();
		GL.BindBufferBase(BufferTargetARB.UniformBuffer, 0, TexturesUniform);
		Span<uint> textures = stackalloc uint[Registry<Tile>.Count];
		for (int i = 0; i < textures.Length; i++)
			textures[i] = (uint)i;
		SetTexturesBuffer(textures);

		// Prepare the shader uniforms
		ProjectionUniform = GL.GetUniformLocation(Shader, "projection");
		ChunkPositionUniform = GL.GetUniformLocation(Shader, "chunk");
		GL.UniformBlockBinding(Shader, GL.GetUniformBlockIndex(Shader, "gs_rendering"), 0);

		// Configure OpenGL
		GL.ClearColor(Color4.Dimgray);
		GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
	}

	public ProgramHandle Shader { get; }
	private int ProjectionUniform { get; }
	private int ChunkPositionUniform { get; }
	private BufferHandle TexturesUniform { get; }

	private Camera _camera;

	public IChunkRenderer Prepare(Chunk chunk) {
		return new GeometryChunkRenderer(this, chunk);
	}

	public void WillUpdate(in Camera camera, in FrameEventArgs frame) {
		// Update the projection if the camera was updated
		if (_camera != camera) {
			GL.UniformMatrix4f(ProjectionUniform, false, camera.View * camera.Projection);
			_camera = camera;
		}

		// Ensure the shader is ready to render
		GL.BindTexture(TextureTarget.Texture2d, Resources.TerrainAtlas);
		GL.UseProgram(Shader);
	}

	public void DidUpdate(in FrameEventArgs frame) {

	}

	#region Uniform Management

	public void SetTexturesBuffer(ReadOnlySpan<uint> buffer) {
		GL.BindBuffer(BufferTargetARB.UniformBuffer, TexturesUniform);
		GL.BufferData(BufferTargetARB.UniformBuffer, buffer, BufferUsageARB.DynamicDraw);
		GL.BindBuffer(BufferTargetARB.UniformBuffer, BufferHandle.Zero);
	}

	public void SetChunkPosition(int x, int y)
		=> GL.Uniform2i(ChunkPositionUniform, x, y);

	public void SetChunkPosition(Vector2i position)
		=> SetChunkPosition(position.X, position.Y);

	#endregion

	public void Dispose() {
		GL.DeleteProgram(Shader);
	}

}
