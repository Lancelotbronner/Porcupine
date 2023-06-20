using Porcupine.Registries;

namespace Porcupine.Worlds;

public sealed partial class Chunk {

	public const int TILE = 16;
	public const int SIZE = TILE * TILE;

	public Chunk(int x, int y) {
		X = x;
		Y = y;
	}

	public static Chunk CreateRandom(int x, int y, Random random, byte range) {
		Chunk chunk = new(x, y);
		chunk.RandomizeTerrain(random, range);
		return chunk;
	}

	#region Chunk Management

	public int X { get; }
	public int Y { get; }

	#endregion

	#region Terrain Management

	public event Action<Chunk>? TerrainChanged;

	internal ChunkTerrainData terrain;

	public ReadOnlySpan<uint> Tiles => terrain.Tiles;

	public uint TileAt(byte index) {
		uint result;
		unsafe {
			result = terrain.tiles[index];
		}
		return result;
	}

	public uint TileAt(byte x, byte y)
		=> unchecked(TileAt((byte)(x * y)));

	public void RandomizeTerrain(Random random, byte range) {
		foreach (ref uint tile in terrain.Tiles)
			tile = (uint)(random.Next() % range);
		OnTerrainChanged();
	}

	public void OnTerrainChanged() {
		TerrainChanged?.Invoke(this);
		WorldEvents.OnTerrainChanged(this);
	}

	#endregion

	#region Loose Items Management

	private readonly List<ChunkPositioned<Stack<Item>>> loose = new();

	#endregion

	public override int GetHashCode()
		=> HashCode.Combine(X, Y);

}

public struct ChunkPositioned<T> {
	public T subject;
	public byte index;
}

internal unsafe struct ChunkTerrainData {
	public fixed uint tiles[Chunk.SIZE];

	public readonly Span<uint> Tiles {
		get {
			fixed (uint* ptr = tiles)
				return new(ptr, Chunk.SIZE);
		}
	}

}
