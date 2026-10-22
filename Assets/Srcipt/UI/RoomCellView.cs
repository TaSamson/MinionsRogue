using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(Button))]
public class RoomCellView : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text label;

    private RoomNode room;
    private Action<RoomNode> onClicked;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(HandleClick);
    }

    public void Setup(RoomNode room, Action<RoomNode> onClicked)
    {
        this.room = room;
        this.onClicked = onClicked;

        if (room == null)
        {
            background.color = new Color(0.2f, 0.2f, 0.2f, 0.4f);
            label.text = "";
            GetComponent<Button>().interactable = false;
            return;
        }

        background.color = GetColor(room.Type);
        label.text = GetSymbol(room.Type);
    }

    private void HandleClick()
    {
        if (room != null) onClicked?.Invoke(room);
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