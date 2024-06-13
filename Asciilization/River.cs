namespace Asciilization;

public class River
{
    public List<Hex> mainHexes;
    public List<Hex> additionalHexes;
    public Dictionary<Hex, int> directions;
    
    public River()
    {
        mainHexes = new List<Hex>();
        additionalHexes = new List<Hex>();
        directions = new Dictionary<Hex, int>();
    }
    
    public void Clear()
    {
        mainHexes.Clear();
        additionalHexes.Clear();
        directions.Clear();
    }
}