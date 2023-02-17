namespace Asciilization;

public class Map
{
    public Hex[,] hexes;
    public Hex isSelected;
    
    public Map(int xLen, int yLen)
    {
        hexes = new Hex[yLen, xLen];
        Fill();
        //Select();
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
        foreach (Hex hex in hexes)
        {
            hex.neighbors = new Neighbors(hex, this);
        }
    }
    
    public void Select(Hex hex)
    {
        isSelected = hexes[hex.coord.y, hex.coord.x];
    }

    // public void Select()
    // {
    //     if (Output.screenSize.x % 2 == 0 && Output.screenSize.y % 2 == 0)
    //     {
    //         isSelected = hexes[hexes.GetLength(0) / 2 + 1, hexes.GetLength(1) / 2 + 1];
    //     }
    //     else if (Output.screenSize.x % 2 == 0)
    //     {
    //         isSelected = hexes[hexes.GetLength(0) / 2, hexes.GetLength(1) / 2 + 1];
    //     }
    //     else if (Output.screenSize.y % 2 == 0)
    //     {
    //         isSelected = hexes[hexes.GetLength(0) / 2 + 1, hexes.GetLength(1) / 2];
    //     }
    //     else
    //     {
    //         isSelected = hexes[hexes.GetLength(0) / 2, hexes.GetLength(1) / 2];
    //     }
    // }
}