namespace Asciilization;

public class Neighbors
{
    public Hex?[] hexes;

    public Neighbors(Hex hex, Map map)
    {
        hexes = new Hex[6];
        Init(hex, map);
    }

    public void Init(Hex hex, Map map)
    {
        int delta;
        if (hex.coord.x % 2 == 0)
        {
            delta = 0;
        }
        else
        {
            delta = 1;
        }
        if (hex.coord.y > 0)
        {
            hexes[0] = map.hexes[hex.coord.y - 1, hex.coord.x];
        }
        if (hex.coord.y - 1 + delta > -1 && hex.coord.x > 0)
        {
            hexes[1] = map.hexes[hex.coord.y - 1 + delta, hex.coord.x - 1];
        }
        if (hex.coord.y + delta < map.hexes.GetLength(0) && hex.coord.x > 0)
        {
            hexes[2] = map.hexes[hex.coord.y + delta, hex.coord.x - 1];
        }
        if (hex.coord.y < map.hexes.GetLength(0) - 1)
        {
            hexes[3] = map.hexes[hex.coord.y + 1, hex.coord.x];
        }
        if (hex.coord.y + delta < map.hexes.GetLength(0) && hex.coord.x < map.hexes.GetLength(1) - 1)
        {
            hexes[4] = map.hexes[hex.coord.y + delta, hex.coord.x + 1];
        }
        if (hex.coord.y - 1 + delta > -1 && hex.coord.x < map.hexes.GetLength(1) - 1)
        {
            hexes[5] = map.hexes[hex.coord.y - 1 + delta, hex.coord.x + 1];
        }
    }
}