namespace Porcupine.Worlds;

public struct Stack<T> {

	public Stack(T subject, uint count = 1) {
		Subject = subject;
		Count = count;
	}

	public required T Subject { get; init; }
	public uint Count { get; init; } = 1;
}
