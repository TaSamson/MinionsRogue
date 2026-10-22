using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MinionsRogue/Minion")]
public class MinionData : ScriptableObject
{
    public string minionName;
    public MinionType type; // Melee, Tank, Ranged, Mage
    public int baseHealth;
    public int baseAttack;
    public int cost;
    public GameObject prefab;
}


public enum MinionType { Melee, Tank, Ranged, Mage }