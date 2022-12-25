namespace Asciilization;

public class Map
{
    public Hex[,] hexes;
    public Hex isSelected;

    public Map(int xLen, int yLen)
    {
        hexes = new Hex[yLen, xLen];
    }

    public void Fill()
    {
        for (int i = 0; i < hexes.GetLength(0); i++)
        {
            for (int j = 0; j < hexes.GetLength(1); j++)
            {
                hexes[i, j] = new Hex(j, i);
            }
        }
        isSelected = hexes[0, 0];
    }
}