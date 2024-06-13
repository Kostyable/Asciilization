namespace Asciilization;

public class Map
{
    public Hex[,] hexes;
    public Hex isSelected;
    
    public Map(int xLen, int yLen)
    {
        hexes = new Hex[yLen, xLen];
        Fill();
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
}