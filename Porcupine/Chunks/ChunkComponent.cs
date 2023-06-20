using Porcupine.Client;
using Porcupine.Worlds;

namespace Porcupine.Chunks;

internal static class ChunkComponent<TComponent> {

	private static readonly Dictionary<Chunk, TComponent> _components = new();

	static ChunkComponent() {
		ChunkManager.Unloaded += OnUnload;
	}

	private static void OnUnload(Chunk chunk) {
		_components.Remove(chunk);
	}

	public static void Associate(Chunk chunk, TComponent component) {
		_components.Add(chunk, component);
	}

	public static void Dissociate(Chunk chunk) {
		_components.Remove(chunk);
	}

}
