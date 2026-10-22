using UnityEngine;

using System.Collections.Generic;

public class RoomNode
{
    public int X;
    public int Y;
    public RoomType Type;
    public bool Visited;
    public List<Direction> Connections = new List<Direction>();

    public RoomNode(int x, int y, RoomType type)
    {
        X = x;
        Y = y;
        Type = type;
    }
}