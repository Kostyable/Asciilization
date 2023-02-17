namespace Asciilization;

public abstract class Unit
{
    public Civilization owner;
    public int hp;
    public Hex currentHex;
    public int movePoints;

    public Unit(Hex current, Civilization owner)
    {
        currentHex = current;
        this.owner = owner;
    }

    public void Move(Hex target)
    {
        if (currentHex.neighbors.hexes.Contains(target))
        {
            currentHex = target;
        }
    }
}