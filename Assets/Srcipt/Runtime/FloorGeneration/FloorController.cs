using UnityEngine;

public class FloorController : MonoBehaviour
{
    [SerializeField] private int seed = 12345;
    [SerializeField] private int floorLevel = 1;
    [SerializeField] private FloorMapView mapView;

    private FloorGrid grid;
    private RoomNode currentRoom;

    private void Start()
    {
        var generator = new FloorGenerator(seed);
        grid = generator.Generate(floorLevel);

        currentRoom = FindStartRoom();
        currentRoom.Visited = true;

        mapView.OnRoomClicked += HandleRoomClicked;
        mapView.Display(grid, currentRoom);
    }

    private void OnDestroy()
    {
        if (mapView != null) mapView.OnRoomClicked -= HandleRoomClicked;
    }

    private RoomNode FindStartRoom()
    {
        for (int x = 0; x < grid.Width; x++)
            for (int y = 0; y < grid.Height; y++)
                if (grid.Rooms[x, y]?.Type == RoomType.Start)
                    return grid.Rooms[x, y];

        return null;
    }

    private void HandleRoomClicked(RoomNode room)
    {
        if (!currentRoom.IsConnectedTo(room)) return;

        bool firstVisit = !room.Visited;

        currentRoom = room;
        room.Visited = true;

        mapView.Display(grid, currentRoom);

        if (firstVisit) TriggerRoomEvent(room);
    }

    private void TriggerRoomEvent(RoomNode room)
    {
        switch (room.Type)
        {
            case RoomType.Monster: Debug.Log("Combat !"); break;
            case RoomType.Boss: Debug.Log("Combat de boss !"); break;
            case RoomType.Treasure: Debug.Log("Trésor !"); break;
            case RoomType.Heal: Debug.Log("Soin !"); break;
            case RoomType.Neutral: Debug.Log("Salle neutre."); break;
        }
    }
}