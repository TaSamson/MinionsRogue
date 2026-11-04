using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CombatTester : MonoBehaviour
{
    [SerializeField] private HeroData hero;
    [SerializeField] private List<MinionData> minions;
    [SerializeField] private List<MonsterData> monsters;

    private void Start()
    {
        var allies = new List<CombatUnit> { CombatUnit.FromHero(hero) };
        allies.AddRange(minions.Select(CombatUnit.FromMinion));
        var enemies = monsters.Select(CombatUnit.FromMonster).ToList();

        NumberDuplicates(allies);
        NumberDuplicates(enemies);

        var result = new CombatResolver().Resolve(allies, enemies);

        var sb = new StringBuilder();
        foreach (var a in result.Log)
        {
            sb.AppendLine($"R{a.Round}  {a.Actor.Name} → {a.Target.Name} : -{a.Damage} PV " +
                          $"({a.TargetHealthAfter}/{a.Target.MaxHealth}){(a.TargetDied ? "  MORT" : "")}");
        }
        sb.AppendLine($"--- {result.Outcome} en {result.Rounds} rounds | or +{result.GoldEarned} | xp +{result.XpEarned}");

        Debug.Log(sb.ToString());
    }

    // Deux "Goblin" deviennent "Goblin 1" et "Goblin 2" pour lire le journal
    private void NumberDuplicates(List<CombatUnit> units)
    {
        foreach (var group in units.GroupBy(u => u.Name).Where(g => g.Count() > 1))
        {
            int i = 1;
            foreach (var u in group) u.Name = $"{u.Name} {i++}";
        }
    }
}