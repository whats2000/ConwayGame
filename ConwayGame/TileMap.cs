using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class TileMap : Godot.TileMap
{
    const int TILE_MAP_SIZE = 32;
    public bool pause = true;
    private bool isLeftClick = false;
    private bool isRightClick = false;
    private int currentCellType = 1;

    List<List<int>> field = new();

    [Export] public int Width;
    [Export] public int Height;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        int width_pixel = Width * TILE_MAP_SIZE;
        int height_pixel = Height * TILE_MAP_SIZE;

        Camera2D camera = GetNode<Camera2D>("../Camera2D");

        camera.Position = new Vector2(width_pixel, height_pixel) / 2;
        camera.Zoom = new Vector2(0.25f, 0.25f);

        for (int x = 0; x < Width; x++)
        {
            field.Add(new List<int>());
            for (int y = 0; y < Height; y++)
            {
                SetCell(0, new Vector2I(x, y), 0, new Vector2I(0, 0));
                field[x].Add(0);
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (StartMenu.isStartMenuOpen || SettingMenu.isSettingMenuOpen) return;

        Vector2 mouse_pos = GetLocalMousePosition() / TILE_MAP_SIZE;
        Vector2I mouse_pos_2i = new((int)mouse_pos.X, (int)mouse_pos.Y);

        PlaceCell(mouse_pos_2i);
        RemoveCell(mouse_pos_2i);

        Process_cell();
    }

    // This function is modify by GPT-4
    // Link for prompt https://chat.openai.com/share/836d6086-fb5c-402e-93fc-c1ca21c2aa22
    private void Process_cell()
    {
        if (pause) return;

        List<List<int>> tempField = new();

        for (int x = 0; x < Width; x++)
        {
            List<int> tempRow = new();

            for (int y = 0; y < Height; y++)
            {
                Dictionary<int, int> neighborCount = new Dictionary<int, int>();

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if ((dx == 0 && dy == 0) ||
                            x + dx < 0 || x + dx >= Width ||
                            y + dy < 0 || y + dy >= Height) continue;

                        int neighborType = field[x + dx][y + dy];
                        if (!neighborCount.ContainsKey(neighborType))
                        {
                            neighborCount[neighborType] = 1;
                        }
                        else
                        {
                            neighborCount[neighborType]++;
                        }
                    }
                }

                int cellType = field[x][y];
                int newCellType = cellType;

                int sameTypeCount = neighborCount.ContainsKey(cellType) ? neighborCount[cellType] : 0;
                int totalNeighbors = neighborCount.Values.Sum();
                int enemyCount = totalNeighbors - sameTypeCount;

                if (cellType == 0)
                {
                    int maxNeighborType = neighborCount.OrderByDescending(kv => kv.Key != 0 ? kv.Value : 0).FirstOrDefault().Key;
                    if (maxNeighborType != 0 && neighborCount[maxNeighborType] >= SettingMenu.minBreedRequired && neighborCount[maxNeighborType] <= SettingMenu.maxBreedRequired && new Random().Next(1, 101) <= SettingMenu.breedChance)
                    {
                        newCellType = maxNeighborType;
                    }
                }
                else
                {
                    if (enemyCount - sameTypeCount >= 3 && new Random().Next(1, 101) <= SettingMenu.attackDead)
                    {
                        newCellType = 0;
                    }
                    else if (sameTypeCount <= SettingMenu.minDeadRequired || sameTypeCount >= SettingMenu.maxDeadRequired && new Random().Next(1, 101) <= SettingMenu.deadChance)
                    {
                        newCellType = 0;
                    }
                }

                SetCell(0, new Vector2I(x, y), newCellType, new Vector2I(0, 0));
                tempRow.Add(newCellType);
            }

            tempField.Add(tempRow);
        }

        field = tempField;
    }
    // End of GPT-4 modify


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

        if (@event.IsActionPressed("previous_cell_type"))
        {
            currentCellType = Math.Max(currentCellType - 1, 1);
        }

        if (@event.IsActionPressed("next_cell_type"))
        {
            currentCellType = Math.Min(currentCellType + 1, SettingMenu.numberOfGroup);
        }

        if (@event.IsActionPressed("random_place_cell"))
        {
            RandomPlaceCell();
        }

        if (@event.IsActionPressed("clear_cell"))
        {
            ClearCell();
        }
    }

    private void ClearCell()
    {
        for (int x = 0; x < Width; x++)
        {
            field.Add(new List<int>());
            for (int y = 0; y < Height; y++)
            {
                if (field[x][y] == 0) continue;

                SetCell(0, new Vector2I(x, y), 0, new Vector2I(0, 0));
                field[x][y] = 0;
            }
        }
    }

    private void RandomPlaceCell()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (new Random().Next(0, 9) == 1)
                {
                    if (field[x][y] > 0) continue;

                    int cell_type = new Random().Next(1, SettingMenu.numberOfGroup + 1);

                    SetCell(0, new Vector2I(x, y), cell_type, new Vector2I(0, 0));
                    field[x][y] = cell_type;
                }
            }
        }
    }

    private void PlaceCell(Vector2I mouse_pos_2i)
    {
        if (mouse_pos_2i.X < 0 ||
            mouse_pos_2i.X >= Width ||
            mouse_pos_2i.Y < 0 ||
            mouse_pos_2i.Y >= Height ||
            field[mouse_pos_2i.X][mouse_pos_2i.Y] > 0 ||
            !isLeftClick) return;

        SetCell(0, mouse_pos_2i, currentCellType, new Vector2I(0, 0));
        field[mouse_pos_2i.X][mouse_pos_2i.Y] = currentCellType;
    }

    private void RemoveCell(Vector2I mouse_pos_2i)
    {
        if (mouse_pos_2i.X < 0 ||
            mouse_pos_2i.X >= Width ||
            mouse_pos_2i.Y < 0 ||
            mouse_pos_2i.Y >= Height ||
            field[mouse_pos_2i.X][mouse_pos_2i.Y] == 0 ||
            !isRightClick) return;

        SetCell(0, mouse_pos_2i, 0, new Vector2I(0, 0));
        field[mouse_pos_2i.X][mouse_pos_2i.Y] = 0;
    }
}
