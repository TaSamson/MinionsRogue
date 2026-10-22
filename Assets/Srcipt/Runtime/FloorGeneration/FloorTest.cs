using UnityEngine;

public class FloorController : MonoBehaviour
{
    [SerializeField] private int seed = 12345;
    [SerializeField] private int floorLevel = 1;
    [SerializeField] private FloorMapView mapView;

    private void Start()
    {
        var generator = new FloorGenerator(seed);
        var grid = generator.Generate(floorLevel);

        mapView.Display(grid);
        mapView.OnRoomClicked += HandleRoomClicked;
    }

    private void HandleRoomClicked(RoomNode room)
    {
        // Pour l'instant on ne fait rien de plus, on branchera Combat/Shop dessus plus tard
    }
}