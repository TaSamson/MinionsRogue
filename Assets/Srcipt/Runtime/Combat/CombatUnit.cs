using System;

public enum Team { Player, Enemy }

public class CombatUnit
{
    public string Name { get; set; }
    public Team Team { get; private set; }
    public MinionType UnitClass { get; private set; }
    public bool IsHero { get; private set; }

    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public int Attack { get; private set; }
    public int Speed { get; private set; }

    public int GoldReward { get; private set; }
    public int XpReward { get; private set; }

    public bool IsAlive => CurrentHealth > 0;

    public CombatUnit(string name, Team team, MinionType unitClass, int maxHealth,
                      int attack, int speed, bool isHero = false, int gold = 0, int xp = 0)
    {
        Name = name;
        Team = team;
        UnitClass = unitClass;
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        Attack = attack;
        Speed = speed;
        IsHero = isHero;
        GoldReward = gold;
        XpReward = xp;
    }

    // Retourne les PV réellement retirés (jamais plus que ce qui reste)
    public int TakeDamage(int amount)
    {
        int dealt = Math.Min(amount, CurrentHealth);
        CurrentHealth -= dealt;
        return dealt;
    }

    public static CombatUnit FromHero(HeroData h)
    {
        // v1 : seul le rôle Tank se comporte en tank, les autres rôles comme de la mêlée
        var unitClass = h.role == HeroRole.Tank ? MinionType.Tank : MinionType.Melee;
        return new CombatUnit(h.heroName, Team.Player, unitClass, h.maxHealth, h.baseAttack, h.speed, true);
    }

    public static CombatUnit FromMinion(MinionData m)
    {
        return new CombatUnit(m.minionName, Team.Player, m.type, m.baseHealth, m.baseAttack, m.speed);
    }

    public static CombatUnit FromMonster(MonsterData m)
    {
        return new CombatUnit(m.monsterName, Team.Enemy, MinionType.Melee, m.maxHealth,
                              m.baseAttack, m.speed, false, m.goldReward, m.xpReward);
    }
}