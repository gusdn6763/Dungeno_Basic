[System.Serializable]
public class Stat
{
    public int health;
    public int strength;
    public int intellect;
    public int agility;

    public Stat()
    {
        health = 0;
        strength = 0;
        intellect = 0;
        agility = 0;
    }

    public Stat(int health, int strength, int intellect, int agility = 0)
    {
        this.health = health;
        this.strength = strength;
        this.intellect = intellect;
        this.agility = agility;
    }

    public static Stat operator +(Stat stat1, Stat stat2)
    {
        return new Stat(
            stat1.health + stat2.health,
            stat1.strength + stat2.strength,
            stat1.intellect + stat2.intellect,
            stat1.agility + stat2.agility
        );
    }
    public static Stat operator -(Stat stat1, Stat stat2)
    {
        return new Stat(
            stat1.health - stat2.health,
            stat1.strength - stat2.strength,
            stat1.intellect - stat2.intellect,
            stat1.agility - stat2.agility
        );
    }
}