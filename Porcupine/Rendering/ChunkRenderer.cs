using OpenTK.Windowing.Common;
using Porcupine.Worlds;

namespace Porcupine.Rendering;

public readonly struct ChunkRenderer : IDisposable {

	public ChunkRenderer(IChunkRendererFactory factory) {
		_factory = factory;
	}

	private readonly IChunkRendererFactory _factory;
	private readonly Dictionary<Chunk, IChunkRenderer> _renderers = new();
	private readonly HashSet<IChunkRenderer> _active = new();

	public readonly IReadOnlyCollection<Chunk> Chunks => _renderers.Keys;

	private readonly IChunkRenderer this[Chunk chunk] {
		get {
			if (_renderers.TryGetValue(chunk, out IChunkRenderer? renderer))
				return renderer;
			return _renderers[chunk] = _factory.Prepare(chunk);
		}
	}

	public readonly void Add(Chunk chunk) {
		if (_renderers.ContainsKey(chunk))
			return;

		_renderers[chunk] = _factory.Prepare(chunk);
	}

	public readonly void Remove(Chunk chunk) {
		if (_renderers.TryGetValue(chunk, out IChunkRenderer? renderer))
			renderer.Dispose();
		_renderers.Remove(chunk);
	}

	public readonly void Activate(Chunk chunk)
		=> _active.Add(this[chunk]);

	public readonly void Deactivate(Chunk chunk)
		=> _active.Remove(this[chunk]);

	public readonly void SetActive(IEnumerable<Chunk> chunks) {
		_active.Clear();
		foreach (Chunk chunk in chunks)
			_active.Add(this[chunk]);
	}

	public readonly void Update(in Camera camera, in FrameEventArgs frame) {
		_factory.WillUpdate(in camera, in frame);

		foreach (IChunkRenderer renderer in _active)
			renderer.Update(in camera, in frame);

		_factory.DidUpdate(in frame);
	}

	public readonly void Dispose() {
		_factory.Dispose();
		foreach (IChunkRenderer renderer in _renderers.Values)
			renderer.Dispose();

		_renderers.Clear();
		_active.Clear();
	}

}