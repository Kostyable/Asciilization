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
        if (Printing.screenSize.x % 2 == 0 && Printing.screenSize.y % 2 == 0)
        {
            isSelected = hexes[hexes.GetLength(0) / 2 + 1, hexes.GetLength(1) / 2 + 1];
        }
        else if (Printing.screenSize.x % 2 == 0)
        {
            isSelected = hexes[hexes.GetLength(0) / 2, hexes.GetLength(1) / 2 + 1];
        }
        else if (Printing.screenSize.y % 2 == 0)
        {
            isSelected = hexes[hexes.GetLength(0) / 2 + 1, hexes.GetLength(1) / 2];
        }
        else
        {
            isSelected = hexes[hexes.GetLength(0) / 2, hexes.GetLength(1) / 2];
        }
        
    }
}