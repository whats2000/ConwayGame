using Godot;

public partial class SettingMenu : Control
{
	private bool isOpen = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Scale = Vector2.One / GetParent<Camera2D>().Zoom;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (StartMenu.isStartMenuOpen) return;

		if (@event.IsActionPressed("game_setting"))
		{
			isOpen = !isOpen;
			Visible = isOpen;
		}
	}

	private void OnContinueButtonPressed()
	{
		isOpen = false;
		Visible = isOpen;
	}


	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}
}
