namespace Asciilization;

public class Hex
{
    public Coordinates coord;
    public Terrain terrain;
    public Civ civ;
    public bool withRiver;
    public int riverDir;
    
    public Hex(int x, int y)
    {
        coord.x = x;
        coord.y = y;
        riverDir = 6;
    }
}