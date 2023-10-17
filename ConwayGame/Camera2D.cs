using Godot;
using System;

public partial class Camera2D : Godot.Camera2D
{
	private Vector2 last_mouse_pos;

	private bool dragging = false;

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("drag_map"))
		{
			last_mouse_pos = GetLocalMousePosition();
			dragging = true;
		}
		else if (@event.IsActionReleased("drag_map"))
		{
			dragging = false;
		}

		if (@event is InputEventMouseMotion motion && dragging)
		{
			Position -= motion.Relative / Zoom;
		}

		if (@event.IsActionPressed("zoom_up"))
		{
			ChangeCameraZoom(1.1f);
		}

		if (@event.IsActionPressed("zoom_down"))
		{
			ChangeCameraZoom(0.9f);
		}
	}

	private void ChangeCameraZoom(float zoom)
	{
		Vector2 newZoom = zoom * Zoom;
		if (newZoom.X < 0.25) return;

		Zoom = newZoom;
	}
}
