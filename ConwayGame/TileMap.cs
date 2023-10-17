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
				temp.Add(0);
			}

			temp_field.Add(temp);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Process_cell();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
			{
				GetTree().Quit();
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("toggle_play"))
		{
			playing = !playing;
		}
		else if (@event.IsActionPressed("place_cell"))
		{
			Vector2 mouse_pos = (GetLocalMousePosition() / TILE_MAP_SIZE).Floor();
			Vector2I mouse_pos_2i = new((int)mouse_pos.X, (int)mouse_pos.Y);

			if (mouse_pos_2i.X >= 0 && mouse_pos_2i.X < Width && mouse_pos_2i.Y >= 0 && mouse_pos_2i.Y < Height)
			{
				if (temp_field[mouse_pos_2i.X][mouse_pos_2i.Y] == 1)
				{
					SetCell(0, mouse_pos_2i, 0, new Vector2I(0, 0));
					temp_field[mouse_pos_2i.X][mouse_pos_2i.Y] = 0;
				}
				else
				{
					SetCell(0, mouse_pos_2i, 1, new Vector2I(0, 0));
					temp_field[mouse_pos_2i.X][mouse_pos_2i.Y] = 1;
				}
			}
		}
	}

	private void Process_cell()
	{
		if (!playing) return;
		List<List<int>> new_field = new();

		for (int x = 0; x < Width; x++)
		{
			List<int> new_row = new();

			for (int y = 0; y < Height; y++)
			{
				int neighbors = Count_neighbors(x, y);
				int current_state = temp_field[x][y];
				int new_state = current_state;

				if (current_state == 1)
				{
					// Cell is alive
					if (neighbors < 2 || neighbors > 3)
					{
						new_state = 0; // Dies due to underpopulation or overpopulation
					}
				}
				else
				{
					// Cell is dead
					if (neighbors == 3)
					{
						new_state = 1; // Becomes alive due to reproduction
					}
				}

				new_row.Add(new_state);
				SetCell(0, new Vector2I(x, y), new_state, new Vector2I(0, 0));
			}

			new_field.Add(new_row);
		}

		temp_field = new_field;
	}

	private int Count_neighbors(int x, int y)
	{
		int count = 0;
		int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
		int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

		for (int i = 0; i < 8; i++)
		{
			int nx = x + dx[i];
			int ny = y + dy[i];

			if (nx >= 0 && nx < Width && ny >= 0 && ny < Height)
			{
				count += temp_field[nx][ny];
			}
		}

		return count;
	}
}
