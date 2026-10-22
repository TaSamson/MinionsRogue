using UnityEngine;
using UnityEngine.UI;
using System;

public class FloorMapView : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup gridLayout;
    [SerializeField] private RoomCellView roomCellPrefab;

    public event Action<RoomNode> OnRoomClicked;

    public void Display(FloorGrid grid)
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
                cell.Setup(room, HandleRoomClicked);
            }
        }
    }

    private void HandleRoomClicked(RoomNode room)
    {
        Debug.Log($"Salle cliquée : ({room.X},{room.Y}) - {room.Type}");
        OnRoomClicked?.Invoke(room);
    }
}