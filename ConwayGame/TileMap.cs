using Godot;
using System.Collections.Generic;

public partial class TileMap : Godot.TileMap
{
    const int TILE_MAP_SIZE = 32;
    bool playing = false;
    List<List<int>> temp_field = new();

    [Export] public int Width;
    [Export] public int Height;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        int width_pixel = Width * TILE_MAP_SIZE;
        int height_pixel = Height * TILE_MAP_SIZE;

        Camera2D camera = GetNode<Camera2D>("Camera2D");

        camera.Position = new Vector2(width_pixel, height_pixel) / 2;
        camera.Zoom = new Vector2(width_pixel / 1920, height_pixel / 1080);

        for (int x = 0; x < Width; x++)
        {
            List<int> temp = new();

            for (int y = 0; y < Height; y++)
            {
                SetCell(0, new Vector2I(x, y), 0, new Vector2I(0, 0));
                temp.Add(1);
            }

            temp_field.Add(temp);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey)
            if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
                GetTree().Quit();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("toggle_play"))
        {
            playing = !playing;
        }
    }
}
