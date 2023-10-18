using Godot;

public partial class StartMenu : Control
{
	public static bool isStartMenuOpen = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Scale = Vector2.One / GetParent<Camera2D>().Zoom;
	}

	private void OnStartButtonPressed()
	{
		Visible = false;
		isStartMenuOpen = false;
	}


	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}

}
