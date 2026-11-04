using UnityEngine;
using UnityEngine.UI;
using System;

public class FloorMapView : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup gridLayout;
    [SerializeField] private RoomCellView roomCellPrefab;

    public event Action<RoomNode> OnRoomClicked;

    public void Display(FloorGrid grid, RoomNode currentRoom)
    {
        foreach (Transform child in gridLayout.transform)
            Destroy(child.gameObject);

        gridLayout.constraintCount = grid.Width;

        for (int y = grid.Height - 1; y >= 0; y--)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                var room = grid.GetRoom(x, y);
                var cell = Instantiate(roomCellPrefab, gridLayout.transform);
                cell.Setup(room, GetState(room, currentRoom), HandleRoomClicked);
            }
        }
    }

    private RoomCellState GetState(RoomNode room, RoomNode currentRoom)
    {
        if (room == null) return RoomCellState.Empty;
        if (room == currentRoom) return RoomCellState.Current;
        if (currentRoom.IsConnectedTo(room)) return RoomCellState.Reachable;
        if (room.Visited) return RoomCellState.Visited;
        return RoomCellState.Hidden;
    }

    private void HandleRoomClicked(RoomNode room)
    {
        OnRoomClicked?.Invoke(room);
    }
}