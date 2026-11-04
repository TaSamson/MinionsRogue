using UnityEngine;

[CreateAssetMenu(menuName = "MinionsRogue/Monster")]
public class MonsterData : ScriptableObject
{
    [Header("Identité")]
    public string monsterName;
    public MonsterCategory category;
    public GameObject prefab;

    [Header("Stats")]
    public int maxHealth = 15;
    public int baseAttack = 4;
    public int speed = 4;

    [Header("Récompenses")]
    public int goldReward = 10;
    public int xpReward = 5;
}