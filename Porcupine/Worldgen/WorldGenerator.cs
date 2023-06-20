using Porcupine.Registries;
using Porcupine.Worlds;

namespace Porcupine.Worldgen;

public sealed class WorldGenerator {

	private readonly Random _random;

	public WorldGenerator(Random seed) {
		_random = seed;
	}

	public void GenerateTerrain(Chunk chunk) {
		foreach (ref uint tile in chunk.terrain.Tiles)
			tile = (uint)(_random.Next() % Registry<Tile>.Count);
		chunk.OnTerrainChanged();
	}

}
