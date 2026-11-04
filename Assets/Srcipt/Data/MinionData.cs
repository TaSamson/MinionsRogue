using UnityEngine;

[CreateAssetMenu(menuName = "MinionsRogue/Minion")]
public class MinionData : ScriptableObject
{
    [Header("Identité")]
    public string minionName;
    public MinionType type;
    public GameObject prefab;

    [Header("Stats")]
    public int baseHealth = 10;
    public int baseAttack = 3;
    public int speed = 5;

    [Header("Coûts")]
    public int summonCost = 1;  // points d'invocation consommés avant un combat
    public int shopPrice = 20;  // or demandé à la boutique
}