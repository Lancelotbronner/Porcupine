using System.ComponentModel;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Porcupine.Client;
using Porcupine.Registries;
using Porcupine.Rendering;
using Porcupine.Worldgen;
using Porcupine.Worlds;

namespace Porcupine;

public sealed class PorcupineWindow : GameWindow {

	public PorcupineWindow(GameWindowSettings game, NativeWindowSettings window)
		: base(game, window) {
		VSync = VSyncMode.Adaptive;

		GL.Enable(EnableCap.Blend);
		GL.Enable(EnableCap.Texture2d);

		window.Flags |= ContextFlags.Default;
	}

	private ChunkRenderer _renderer;
	private Camera _camera = new();

	protected override void OnLoad() {
		base.OnLoad();

		// Configure the viewport
		_camera.SetViewport(Size.X, Size.Y);
		GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

		// Configure registries
		//TODO: Use ClientRegistry<Tile> to register UVs and tint color?
		Registry<Tile>.Add("dirt", new());
		Registry<Tile>.Add("sand", new());
		Registry<Tile>.Add("wilderness", new());
		Registry<Tile>.Add("grass", new());

		// Configure the renderer
		_renderer = new(new GeometryChunkRendererFactory());
		ChunkManager.ActiveChunksChanged += _renderer.SetActive;

		// Configure world generation
		Random random = Random.Shared;
		WorldGenerator worldgen = new(random);
		ChunkManager.Generate += worldgen.GenerateTerrain;

		//HACK: Chunk population
		ChunkManager.Activate(new(0, 0));
		ChunkManager.Activate(new(0, 1));
		ChunkManager.Activate(new(1, 0));
		ChunkManager.Activate(new(1, 1));
	}

	protected override void OnUpdateFrame(FrameEventArgs e) {
		base.OnUpdateFrame(e);

		if (KeyboardState.IsKeyDown(Keys.Escape))
			Close();

		//HACK: Camera controls
		_camera.Zoom += MouseState.ScrollDelta.Y / 4;
		if (MouseState.IsButtonDown(MouseButton.Left)) {
			Vector2 delta = MouseState.Delta * (float)e.Time;
			delta.X = -delta.X;
			delta *= 2;
			_camera.Move(delta);
		}

		//HACK: Randomize controls
		if (KeyboardState.IsKeyPressed(Keys.Space))
			foreach (Chunk chunk in ChunkManager.ActiveChunks)
				chunk.RandomizeTerrain(Random.Shared, 4);
	}

	protected override void OnRenderFrame(FrameEventArgs e) {
		base.OnRenderFrame(e);

		GL.Clear(ClearBufferMask.ColorBufferBit);

		_renderer.Update(in _camera, in e);
		RenderingManager.Frame(_camera, e);

		SwapBuffers();
	}

	protected override void OnResize(ResizeEventArgs e) {
		base.OnResize(e);

		GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

		_camera.SetViewport(Size.X, Size.Y);
	}

	protected override void OnClosing(CancelEventArgs e) {
		base.OnClosing(e);

		_renderer.Dispose();
	}

}
