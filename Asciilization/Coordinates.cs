namespace Asciilization;

public struct Coordinates
{
    public int x;
    public int y;
        
    public Coordinates(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    
    public static bool operator ==(Coordinates a, Coordinates b)
    {
        return a.Equals(b);
    }
    
    public static bool operator !=(Coordinates a, Coordinates b)
    {
        return !a.Equals(b);
    }
}