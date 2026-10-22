using UnityEngine;
public class FloorGrid
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public RoomNode[,] Rooms { get; private set; }

    public FloorGrid(int width, int height)
    {
        Width = width;
        Height = height;
        Rooms = new RoomNode[width, height];
    }

    public bool IsInBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

    public RoomNode GetRoom(int x, int y)
    {
        if (!IsInBounds(x, y)) return null;
        return Rooms[x, y];
    }
}