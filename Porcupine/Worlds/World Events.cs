namespace Porcupine.Worlds;

public partial class WorldEvents {

	#region Chunk Events

	public static event Action<Chunk>? TerrainChanged;

	internal static void OnTerrainChanged(Chunk chunk)
		=> TerrainChanged?.Invoke(chunk);

	#endregion

}
