namespace Asciilization;

public class Hex
{
    public Coord coord;
    public Terrain terrain;
    public CivColors civColor;
    public Neighbors? neighbors;
    public bool withRiver;
    public int riverDir;
    
    public Hex(int x, int y)
    {
        coord = new Coord(x, y);
        riverDir = 6;
    }
}