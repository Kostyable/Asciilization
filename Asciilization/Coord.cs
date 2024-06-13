namespace Asciilization;

public struct Coord
{
    public int x;
    public int y;
    
    public Coord(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    
    public static bool operator ==(Coord a, Coord b)
    {
        return a.Equals(b);
    }
    
    public static bool operator !=(Coord a, Coord b)
    {
        return !a.Equals(b);
    }
}