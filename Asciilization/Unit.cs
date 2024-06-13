namespace Asciilization;

public abstract class Unit
{
    public Civilization owner;
    public Hex currentHex;

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