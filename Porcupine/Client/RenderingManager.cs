using OpenTK.Windowing.Common;
using Porcupine.Rendering;
using Porcupine.Worlds;

namespace Porcupine.Client;

public delegate void FrameRenderingHandler(in Camera camera, FrameEventArgs frame);
public delegate void ChunkRenderingHandler(Chunk chunk, in Camera camera, FrameEventArgs frame);

public static class RenderingManager {

	public static event FrameRenderingHandler? WillRender;
	public static event ChunkRenderingHandler? Chunk;
	public static event FrameRenderingHandler? DidRender;

	internal static void Frame(in Camera camera, FrameEventArgs frame) {
		WillRender?.Invoke(camera, frame);
		if (Chunk is not null)
			foreach (Chunk chunk in ChunkManager.ActiveChunks)
				Chunk?.Invoke(chunk, camera, frame);
		DidRender?.Invoke(camera, frame);
	}

}
