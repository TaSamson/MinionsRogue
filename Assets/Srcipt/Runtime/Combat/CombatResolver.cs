using System.Collections.Generic;
using System.Linq;

public enum CombatOutcome { Victory, Defeat, Draw }

public class CombatAction
{
    public int Round;
    public CombatUnit Actor;
    public CombatUnit Target;
    public int Damage;
    public int TargetHealthAfter;
    public bool TargetDied;
}

public class CombatResult
{
    public CombatOutcome Outcome;
    public int Rounds;
    public List<CombatAction> Log = new List<CombatAction>();
    public int GoldEarned;
    public int XpEarned;
}

public class CombatResolver
{
    private const int MaxRounds = 100; // sécurité contre les combats infinis

    // Ordre des listes : la première unité est la plus "en avant" (cible des unités de mêlée)
    public CombatResult Resolve(List<CombatUnit> allies, List<CombatUnit> enemies)
    {
        var result = new CombatResult();

        var all = new List<CombatUnit>();
        all.AddRange(allies);
        all.AddRange(enemies);

        int round = 0;

        while (round < MaxRounds && !PlayerLost(allies) && !AllDead(enemies))
        {
            round++;

            // Initiative : vitesse décroissante. Le tri est stable, donc à vitesse égale
            // les alliés (placés en premier dans 'all') jouent avant les ennemis.
            var order = all.Where(u => u.IsAlive).OrderByDescending(u => u.Speed).ToList();

            foreach (var actor in order)
            {
                if (!actor.IsAlive) continue; // mort pendant ce round

                var foes = actor.Team == Team.Player ? enemies : allies;
                var target = PickTarget(actor, foes);
                if (target == null) break;

                int dealt = target.TakeDamage(actor.Attack);

                result.Log.Add(new CombatAction
                {
                    Round = round,
                    Actor = actor,
                    Target = target,
                    Damage = dealt,
                    TargetHealthAfter = target.CurrentHealth,
                    TargetDied = !target.IsAlive
                });

                if (PlayerLost(allies) || AllDead(enemies)) break;
            }
        }

        result.Rounds = round;

        if (AllDead(enemies)) result.Outcome = CombatOutcome.Victory;
        else if (PlayerLost(allies)) result.Outcome = CombatOutcome.Defeat;
        else result.Outcome = CombatOutcome.Draw;

        if (result.Outcome == CombatOutcome.Victory)
        {
            result.GoldEarned = enemies.Sum(e => e.GoldReward);
            result.XpEarned = enemies.Sum(e => e.XpReward);
        }

        return result;
    }

    private CombatUnit PickTarget(CombatUnit actor, List<CombatUnit> foes)
    {
        var alive = foes.Where(u => u.IsAlive).ToList();
        if (alive.Count == 0) return null;

        switch (actor.UnitClass)
        {
            case MinionType.Ranged: return alive.OrderBy(u => u.CurrentHealth).First();         // vise le plus fragile
            case MinionType.Mage: return alive.OrderByDescending(u => u.Attack).First();      // vise le plus dangereux
            default: return alive[0];                                            // mêlée / tank : l'avant-ligne
        }
    }

    private bool AllDead(List<CombatUnit> units) => !units.Any(u => u.IsAlive);

    // Le joueur perd si le héros tombe ou si toute son équipe est morte
    private bool PlayerLost(List<CombatUnit> allies) =>
        AllDead(allies) || allies.Any(u => u.IsHero && !u.IsAlive);
}