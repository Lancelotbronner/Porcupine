using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Porcupine;

NativeWindowSettings settings = new() {
	Size = new Vector2i(800, 600),
	Title = "Project Porcupine",
	// This is needed to run on macos
	Flags = ContextFlags.ForwardCompatible,
};

#if DEBUG
settings.Flags |= ContextFlags.Debug;
#endif

// To create a new window, create a class that extends GameWindow, then call Run() on it.
using (PorcupineWindow window = new(GameWindowSettings.Default, settings))
	window.Run();
