using OpenTK.Windowing.Common;
using Porcupine.Worlds;

namespace Porcupine.Rendering;

public interface IChunkRenderer: IDisposable {

	/// <summary>
	/// Draw the chunk.
	/// </summary>
	/// <param name="camera">The camera through which to draw the chunk</param>
	abstract void Update(in Camera camera, in FrameEventArgs frame);

}

public interface IChunkRendererFactory: IDisposable {

	/// <summary>
	/// Called to prepare the chunk for rendering in the background as a mean
	/// to speed up the initial rendering time.
	/// </summary>
	/// <param name="chunk">The chunk which will eventually be rendered</param>
	abstract IChunkRenderer Prepare(Chunk chunk);

	abstract void WillUpdate(in Camera camera, in FrameEventArgs frame);
	abstract void DidUpdate(in FrameEventArgs frame);

}
