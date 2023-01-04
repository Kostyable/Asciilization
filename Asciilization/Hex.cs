namespace Asciilization;

public class Hex
{
    public Coordinates coordinates;
    public Terrain terrain;
    public Civ civ;
    public bool withRiver;
    public int riverDir;

    public Hex(int x, int y)
    {
        coordinates.x = x;
        coordinates.y = y;
        riverDir = 6;
    }
}