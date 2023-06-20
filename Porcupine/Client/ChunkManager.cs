using System.Diagnostics;
using System.Runtime.InteropServices;
using Porcupine.Worlds;

namespace Porcupine.Client;

// Idea:
// Whenever the chunk manager needs to load a new chunk, it receives
// the Chunk struct (key). With it it'll check for serialized chunk data which
// it will load.
//
// Deserialize will be invoked to allow systems to load their component's data
// from the chunk and store it.
//
// Loaded will be invoked to add the chunk to active simulation systems.
//
// Eventually when the chunk is no longer needed, Unload will be invoked to remove
// it from every systems.
//
// If the chunk needs to be written to disk then Serialize will be invoked.

//TODO: Include a random with the chunk's generation seed
public delegate void ChunkGenerationHandler(Chunk chunk);

public delegate void ChunkLoadingHandler(Chunk chunk);
public delegate void ChunkUnloadingHandler(Chunk chunk);

public static class ChunkManager {

	/// <summary>
	/// Invoked whenever a new chunk has to be generated
	/// </summary>
	public static event ChunkGenerationHandler? Generate;

	public static event ChunkLoadingHandler? Loaded;
	public static event ChunkLoadingHandler? Unloaded;

	/// <summary>
	/// Invoked whenever the set of active chunks changed
	/// </summary>
	public static event Action<IReadOnlyCollection<Chunk>>? ActiveChunksChanged;

	private static readonly Dictionary<Chunk, ManagedChunk> _chunks = new();
	private static readonly HashSet<Chunk> _active = new();

	public static IReadOnlyCollection<Chunk> ActiveChunks => _active;

	public static bool EnsureLoaded(Chunk chunk) {
		ref ManagedChunk value = ref CollectionsMarshal.GetValueRefOrAddDefault(_chunks, chunk, out bool exists);
		if (exists)
			return false;

		Debug.WriteLine($"[ChunkManager] Loading and generating chunk at X {chunk.X} Y {chunk.Y}");
		// Try to fetch the serialized chunk
		// Then: Deserialize the chunk
		// Else: Generate the chunk
		Generate?.Invoke(chunk);
		value.flags |= ChunkFlags.Loaded | ChunkFlags.Generated;

		Loaded?.Invoke(chunk);
		return true;
	}

	public static void EnsureUnloaded(Chunk chunk) {
		Deactivate(chunk);
		_chunks.Remove(chunk);
		Unloaded?.Invoke(chunk);
		// If the chunk should be persisted AND it is modified:
		// Invoke Serialize
	}

	public static void Activate(Chunk chunk) {
		EnsureLoaded(chunk);
		if (_active.Add(chunk))
			ActiveChunksChanged?.Invoke(_active);
	}

	public static void Deactivate(Chunk chunk) {
		if (_active.Remove(chunk))
			ActiveChunksChanged?.Invoke(_active);
	}

}

internal struct ManagedChunk {
	public ChunkFlags flags;
}

[Flags]
internal enum ChunkFlags : byte {
	/// <summary>
	/// Wether the chunk finished loading its data
	/// </summary>
	Loaded = 0x1,

	/// <summary>
	/// Wether the chunk finished generating
	/// </summary>
	Generated = 0x2,

	/// <summary>
	/// Wether the chunk was modified during gameplay
	/// </summary>
	Modified = 0x4,

	/// <summary>
	/// Wether the chunk should be persisted to disk
	/// </summary>
	Persistable = 0x8,
}
