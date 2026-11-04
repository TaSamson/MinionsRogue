using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum RoomCellState { Empty, Hidden, Visited, Reachable, Current }

[RequireComponent(typeof(Button))]
public class RoomCellView : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text label;

    private RoomNode room;
    private Action<RoomNode> onClicked;
    private bool clickable;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(HandleClick);
    }

    public void Setup(RoomNode room, RoomCellState state, Action<RoomNode> onClicked)
    {
        this.room = room;
        this.onClicked = onClicked;
        clickable = state == RoomCellState.Reachable;

        switch (state)
        {
            case RoomCellState.Empty:
                background.color = new Color(0.2f, 0.2f, 0.2f, 0.4f);
                label.text = "";
                break;

            case RoomCellState.Hidden:
                background.color = new Color(0.25f, 0.25f, 0.25f, 1f);
                label.text = "?";
                break;

            case RoomCellState.Visited:
                background.color = Dim(GetColor(room.Type), 0.45f);
                label.text = GetSymbol(room.Type);
                break;

            case RoomCellState.Reachable:
                background.color = GetColor(room.Type);
                label.text = GetSymbol(room.Type);
                break;

            case RoomCellState.Current:
                background.color = GetColor(room.Type);
                label.text = "YOU";
                break;
        }
    }

    private void HandleClick()
    {
        if (clickable && room != null) onClicked?.Invoke(room);
    }

    // On n'utilise pas Color * float : ça multiplierait aussi l'alpha
    private Color Dim(Color c, float factor)
    {
        return new Color(c.r * factor, c.g * factor, c.b * factor, 1f);
    }

    private Color GetColor(RoomType type)
    {
        return type switch
        {
            RoomType.Start => new Color(0f, 0.75f, 1f),
            RoomType.Boss => new Color(1f, 0f, 0f),
            RoomType.Treasure => new Color(1f, 0.84f, 0f),
            RoomType.Monster => new Color(1f, 0.55f, 0f),
            RoomType.Heal => new Color(0.2f, 0.8f, 0.2f),
            RoomType.Neutral => new Color(0.8f, 0.8f, 0.8f),
            _ => Color.white
        };
    }

    private string GetSymbol(RoomType type)
    {
        return type switch
        {
            RoomType.Start => "S",
            RoomType.Boss => "B",
            RoomType.Treasure => "T",
            RoomType.Monster => "M",
            RoomType.Heal => "H",
            RoomType.Neutral => "N",
            _ => "?"
        };
    }
}