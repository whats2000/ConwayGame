using Godot;
using System.Collections.Generic;

public partial class TileMap : Godot.TileMap
{
	const int TILE_MAP_SIZE = 32;
	private bool pause = true;
	private bool isLeftClick = false;
	private bool isRightClick = false;

	List<List<bool>> field = new();

	[Export] public int Width;
	[Export] public int Height;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		int width_pixel = Width * TILE_MAP_SIZE;
		int height_pixel = Height * TILE_MAP_SIZE;

		Camera2D camera = GetNode<Camera2D>("Camera2D");

		camera.Position = new Vector2(width_pixel, height_pixel) / 2;
		camera.Zoom = new Vector2(0.25f, 0.25f);

		for (int x = 0; x < Width; x++)
		{
			field.Add(new List<bool>());
			for (int y = 0; y < Height; y++)
			{
				SetCell(0, new Vector2I(x, y), 0, new Vector2I(0, 0));
				field[x].Add(false);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 mouse_pos = GetLocalMousePosition() / TILE_MAP_SIZE;
		Vector2I mouse_pos_2i = new((int)mouse_pos.X, (int)mouse_pos.Y);

		PlaceCell(mouse_pos_2i);
		RemoveCell(mouse_pos_2i);

		Process_cell();
	}


	private void Process_cell()
	{
		if (pause) return;

		List<List<bool>> temp_field = new List<List<bool>>();

		for (int x = 0; x < Width; x++)
		{
			List<bool> temp_row = new List<bool>();

			for (int y = 0; y < Height; y++)
			{
				int count = 0;

				for (int dx = -1; dx <= 1; dx++)
				{
					for (int dy = -1; dy <= 1; dy++)
					{
						if ((dx == 0 && dy == 0) ||
							x + dx < 0 || x + dx >= Width ||
							y + dy < 0 || y + dy >= Height) continue;

						if (!field[x + dx][y + dy]) continue;
						count++;
					}
				}

				if (!field[x][y])
				{
					if (count == 3)
					{
						SetCell(0, new Vector2I(x, y), 1, new Vector2I(0, 0));
						temp_row.Add(true);
					}
					else
					{
						temp_row.Add(false);
					}
				}
				else
				{
					if (count < 2 || count > 3)
					{
						SetCell(0, new Vector2I(x, y), 0, new Vector2I(0, 0));
						temp_row.Add(false);
					}
					else
					{
						temp_row.Add(true);
					}
				}
			}

			temp_field.Add(temp_row);
		}

		field = temp_field;
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
			pause = !pause;
			return;
		}

		if (@event.IsActionPressed("place_cell") && !@event.IsActionPressed("drag_map"))
		{
			isLeftClick = true;
		}
		else if (@event.IsActionReleased("place_cell"))
		{
			isLeftClick = false;
		}

		if (@event.IsActionPressed("remove_cell"))
		{
			isRightClick = true;
		}
		else if (@event.IsActionReleased("remove_cell"))
		{
			isRightClick = false;
		}
	}

	private void PlaceCell(Vector2I mouse_pos_2i)
	{
		if (mouse_pos_2i.X < 0 ||
			mouse_pos_2i.X >= Width ||
			mouse_pos_2i.Y < 0 ||
			mouse_pos_2i.Y >= Height ||
			field[mouse_pos_2i.X][mouse_pos_2i.Y] ||
			!isLeftClick) return;

		SetCell(0, mouse_pos_2i, 1, new Vector2I(0, 0));
		field[mouse_pos_2i.X][mouse_pos_2i.Y] = true;
	}

	private void RemoveCell(Vector2I mouse_pos_2i)
	{
		if (mouse_pos_2i.X < 0 ||
			mouse_pos_2i.X >= Width ||
			mouse_pos_2i.Y < 0 ||
			mouse_pos_2i.Y >= Height ||
			!field[mouse_pos_2i.X][mouse_pos_2i.Y] ||
			!isRightClick) return;

		SetCell(0, mouse_pos_2i, 0, new Vector2I(0, 0));
		field[mouse_pos_2i.X][mouse_pos_2i.Y] = false;
	}
}
