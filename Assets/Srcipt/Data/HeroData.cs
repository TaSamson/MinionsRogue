using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MinionsRogue/Hero")]
public class HeroData : ScriptableObject
{
    [Header("Identité")]
    public string heroName;
    public HeroRole role;
    public GameObject prefab;

    [Header("Stats")]
    public int maxHealth = 40;
    public int baseAttack = 5;
    public int speed = 5;

    [Header("Invocation")]
    public int summonPoints = 4; // budget d'invocation par combat
    public List<MinionData> startingMinions;

    [Header("Capacité spéciale")]
    public string abilityName;
    [TextArea] public string abilityDescription;
}