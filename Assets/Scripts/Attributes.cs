[System.Serializable]
public class Attributes
{
    public float fatigue;
    public float hungry;
    public float time;
    public Attributes()
    {
        fatigue = 0;
        hungry = 0;
        time = 0;
    }

    public Attributes(float fatigue, float hungry, float time = 0)
    {
        this.fatigue = fatigue;
        this.hungry = hungry;
        this.time = time;
    }

    public Attributes(Attributes other)
    {
        this.fatigue = other.fatigue;
        this.hungry = other.hungry;
        this.time = other.time;
    }

    public static Attributes operator +(Attributes stat1, Attributes stat2)
    {
        return new Attributes(
            stat1.fatigue + stat2.fatigue,
            stat1.hungry + stat2.hungry,
            stat1.time + stat2.time
        );
    }
    public static Attributes operator -(Attributes stat1, Attributes stat2)
    {
        return new Attributes(
            stat1.fatigue - stat2.fatigue,
            stat1.hungry - stat2.hungry,
            stat1.time - stat2.time
        );
    }
}