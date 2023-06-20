using OpenTK.Mathematics;

namespace Porcupine.Rendering;

public record struct Camera {

	public const float MinimumZoom = 1;
	public const float MaximumZoom = 32;

	public Camera() {
		_position = Vector2.Zero;
		_rotation = 0;
		_zoom = 1;
		_viewport = Vector2i.Zero;
	}

	#region Position Management

	private Vector2 _position;

	/// <summary>
	/// The camera's world position
	/// </summary>
	public Vector2 Position {
		readonly get => _position;
		set => _position = value;
	}

	public float X {
		readonly get => _position.X;
		set => _position.X = value;
	}

	public float Y {
		readonly get => _position.Y;
		set => _position.Y = value;
	}

	public void CenterOn(Vector2 center) {
		Position = center;
	}

	public void Move(Vector2 delta) {
		Position += delta * MaximumZoom / Zoom;
	}

	#endregion

	#region Zoom Management

	private float _zoom;

	/// <summary>
	/// Camera's scaling
	/// </summary>
	public float Zoom {
		readonly get => _zoom;
		set => _zoom = Math.Clamp(value, MinimumZoom, MaximumZoom);
	}

	#endregion

	#region Rotation Management

	private float _rotation;

	/// <summary>
	/// The camera's rotation in radians
	/// </summary>
	public float Rotation {
		readonly get => _rotation;
		set => _rotation = value;
	}

	#endregion

	#region Viewport Management

	private Vector2i _viewport;

	public Vector2i Viewport {
		readonly get => _viewport;
		set => _viewport = value;
	}

	public int Width {
		readonly get => _viewport.X;
		set => _viewport.X = value;
	}

	public int Height {
		readonly get => _viewport.Y;
		set => _viewport.Y = value;
	}

	public void SetViewport(int width, int height) {
		_viewport = new(width, height);
	}

	#endregion

	#region Projection Matrix

	public readonly Matrix4 View => Matrix4.LookAt(_position.X, _position.Y, 1, _position.X, _position.Y, -1, 0, 1, 0);
	public readonly Matrix4 Projection => Matrix4.CreateOrthographic(Width / Zoom, Height / Zoom, 1, -1);

	public readonly Vector2 ScreenToWorld(Vector2 screen) {
		Matrix4 transform = View.Inverted() * Projection.Inverted();
		Vector3 world = Vector3.Unproject(new(screen, 1), 0, 0, Width, Height, -1, 1, transform);
		return world.Xy;
	}

	public readonly Vector2 WorldToScreen(Vector2 world) {
		Matrix4 transform = View * Projection;
		Vector3 screen = Vector3.Project(new(world, 1), 0, 0, Width, Height, -1, 1, transform);
		return screen.Xy;
	}

	#endregion

	public override readonly string ToString()
		=> $"({X:f1} {Y:f1} {Zoom:f1}x {Rotation * 180 / Math.PI:f1}°)";

}
