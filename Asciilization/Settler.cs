namespace Asciilization;

public class Settler : Unit
{
    public Settler(Hex current, Civilization owner) : base(current, owner){}
    public void BuildCity()
    {
        if (currentHex.terrain != Terrain.Water && currentHex.terrain != Terrain.Mountains && currentHex.civColor == CivColors.Without)
        {
            currentHex.civColor = (CivColors)owner.name + 1;
        }
    }
}