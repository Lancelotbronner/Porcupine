using System.Diagnostics;

namespace Porcupine.Registries;

public static class Registry<TSubject> {

	private static readonly Dictionary<string, uint> _mappings = new();
	private static readonly List<TSubject> _contents = new();

	public static IReadOnlyCollection<TSubject> Contents => _contents;
	public static int Count => _contents.Count;

	public static void Add(string identifier, TSubject subject) {
		//HACK: Throw exception on duplicate identifier
		_mappings[identifier] = (uint)_contents.Count;
		_contents.Add(subject);
		WriteDebug($"Registered {identifier}");
	}

	public static uint Resolve(string identifier) {
		return _mappings[identifier];
	}

	public static TSubject Fetch(uint id) {
		return _contents[(int)id];
	}

	private static void WriteDebug(string message)
		=> Debug.WriteLine($"[Registry<{typeof(TSubject).Name}>] {message}");

}
