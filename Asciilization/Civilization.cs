namespace Asciilization;

public class Civilization
{
    public CivNames name;
    public List<Unit> units;

    public Civilization(CivNames name)
    {
        this.name = name;
        units = new List<Unit>();
    }
}