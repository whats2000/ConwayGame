using System;
using Godot;

public partial class SettingMenu : Control
{
    public static bool isSettingMenuOpen = false;
    public static int minDeadRequired = 1;
    public static int maxDeadRequired = 4;
    public static int minBreedRequired = 3;
    public static int maxBreedRequired = 3;

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
            isSettingMenuOpen = !isSettingMenuOpen;
            Visible = isSettingMenuOpen;
        }
    }

    private void OnContinueButtonPressed()
    {
        isSettingMenuOpen = false;
        Visible = isSettingMenuOpen;
    }


    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }

    private void MinDeadSliderChange(double value)
    {
        minDeadRequired = Math.Min((int)value, maxDeadRequired);
    }

    private void MaxDeadSliderChange(double value)
    {
        maxDeadRequired = Math.Max((int)value, minDeadRequired);
    }

    private void MinBreedSliderChange(double value)
    {
        minBreedRequired = Math.Min((int)value, maxBreedRequired);
    }

    private void MaxBreedSliderChange(double value)
    {
        maxBreedRequired = Math.Max((int)value, minBreedRequired);
    }
}
