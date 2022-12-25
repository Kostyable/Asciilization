namespace Asciilization;

public class Generation
{
    public static float[,] valueAlitude;
    public static float[,] valueRainfall;
    public static Random random = new Random();
    public static float sid = random.Next(1, 9999999);
    public static float zoom = 10f;
    static Perlin altitude = new Perlin();
    static Perlin rainfall = new Perlin();
    public static void Map(Map map)
    {
        valueAlitude = new float[map.hexes.GetLength(0), map.hexes.GetLength(1)];
        float maxValue = 0f;
        for (int y = 0; y < map.hexes.GetLength(0); y++)
        {
            for (int x = 0; x < map.hexes.GetLength(1); x++)
            {
                float nx = (x + sid) / zoom + 0.5f, ny = (y + sid) / zoom + 0.5f;
                valueAlitude[y, x] = (altitude.Noise(nx, ny) + 1f) / 2f + 0.1f - 0.3f * (float)Math.Pow(Distance(map, x, y), 2f);
                if (valueAlitude[y, x] < 0f)
                {
                    valueAlitude[y, x] = 0f;
                }
                if (valueAlitude[y, x] > maxValue)
                {
                    maxValue = valueAlitude[y, x];
                }
            }
        }
        sid = random.Next(1, 9999999);
        valueRainfall = new float[map.hexes.GetLength(0), map.hexes.GetLength(1)];
        for (int y = 0; y < map.hexes.GetLength(0); y++)
        {
            for (int x = 0; x < map.hexes.GetLength(1); x++)
            {
                float nx = (x + sid) / zoom + 0.5f, ny = (y + sid) / zoom + 0.5f;
                valueRainfall[y, x] = (rainfall.Noise(nx, ny) + 1f) / 2f;
                if (valueRainfall[y, x] < 0f)
                {
                    valueRainfall[y, x] = 0f;
                }
            }
        }
        for (int y = 0; y < map.hexes.GetLength(0); y++)
        {
            for (int x = 0; x < map.hexes.GetLength(1); x++)
            {
                if (valueAlitude[y, x] < 0.45f)
                {
                    map.hexes[y, x].terrain = Terrain.Water;
                }
                else if (valueAlitude[y, x] > maxValue - 0.1f)
                {
                    if (valueAlitude[y, x] < maxValue - 0.05f)
                    {
                        if (valueRainfall[y, x] < 0.38f)
                        {
                            map.hexes[y, x].terrain = Terrain.DesertHills;
                        }
                        else
                        {
                            map.hexes[y, x].terrain = Terrain.PlainHills;
                        }
                    }
                    else
                    {
                        map.hexes[y, x].terrain = Terrain.Mountains;
                    }

                }
                else
                {
                    if (valueRainfall[y, x] < 0.38f)
                    {
                        map.hexes[y, x].terrain = Terrain.Desert;
                    }
                    else if (valueRainfall[y, x] > 0.58f)
                    {
                        map.hexes[y, x].terrain = Terrain.Forest;
                    }
                    else
                    {
                        map.hexes[y, x].terrain = Terrain.Plain;
                    }
                }
            }
        }
        Frame(map);
    }
    
    public static double Distance(Map map, int x, int y)
    {
        double distX = Math.Abs(x - map.hexes.GetLength(1) / 2 + 1) / (double)(map.hexes.GetLength(1) / 2);
        double distY = Math.Abs(y - map.hexes.GetLength(0) / 2 + 1) / (double)(map.hexes.GetLength(0) / 2);
        return Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2));
    }

    public static void Frame(Map map)
    {
        for (int i = 0; i < map.hexes.GetLength(0); i++)
        {
            map.hexes[i, 0].terrain = Terrain.Water;
            map.hexes[i, map.hexes.GetLength(1) - 1].terrain = Terrain.Water;
        }
        for (int j = 0; j < map.hexes.GetLength(1); j++)
        {
            map.hexes[0, j].terrain = Terrain.Water;
            map.hexes[map.hexes.GetLength(0) - 1, j].terrain = Terrain.Water;
        }
    }

    public static void Rivers(Map map)
    {
        int count = random.Next(6, 11);
        int k = 0;
        while (k < count)
        {
            int i = random.Next(map.hexes.GetLength(0));
            int j = random.Next(map.hexes.GetLength(1));
            if (map.hexes[i, j].terrain == Terrain.Mountains || map.hexes[i, j].terrain == Terrain.PlainHills || map.hexes[i, j].terrain == Terrain.DesertHills)
            {
                map.hexes[i, j].withRiver = true;
            }
        }
    }

    public static void SetRiverDirection(Hex hex, Map map)
    {
        Dictionary<Hex, float> values;
        int maxValue = 0;
        int maxHexIndex;
        if (hex.x % 2 == 0)
        {
            values = new Dictionary<Hex, float>()
            {
                { map.hexes[hex.y - 1, hex.x], valueAlitude[hex.y - 1, hex.x] },
                { map.hexes[hex.y - 1, hex.x - 1], valueAlitude[hex.y - 1, hex.x - 1] },
                { map.hexes[hex.y, hex.x - 1], valueAlitude[hex.y, hex.x - 1] },
                { map.hexes[hex.y + 1, hex.x], valueAlitude[hex.y + 1, hex.x] },
                { map.hexes[hex.y, hex.x + 1], valueAlitude[hex.y, hex.x + 1] },
                { map.hexes[hex.y - 1, hex.x + 1], valueAlitude[hex.y - 1, hex.x + 1] }
            };
        }
        else
        {
            values = new Dictionary<Hex, float>()
            {
                { map.hexes[hex.y - 1, hex.x], valueAlitude[hex.y - 1, hex.x] },
                { map.hexes[hex.y, hex.x - 1], valueAlitude[hex.y, hex.x - 1] },
                { map.hexes[hex.y + 1, hex.x - 1], valueAlitude[hex.y + 1, hex.x - 1] },
                { map.hexes[hex.y + 1, hex.x], valueAlitude[hex.y + 1, hex.x] },
                { map.hexes[hex.y + 1, hex.x + 1], valueAlitude[hex.y + 1, hex.x + 1] },
                { map.hexes[hex.y, hex.x + 1], valueAlitude[hex.y, hex.x + 1] }
            };
        }
    }

    public static void Civs(Map map, int count)
    {
        int i = 0;
        int randX;
        int randY;
        while (i < count)
        {
            randX = random.Next(map.hexes.GetLength(1));
            randY = random.Next(map.hexes.GetLength(0));
            if ((map.hexes[randY, randX].terrain != Terrain.Water) && (map.hexes[randY, randX].terrain != Terrain.Mountains) && (map.hexes[randY, randX].civ == Civ.Without))
            {
                map.hexes[randY, randX].civ = (Civ)i + 1;
                i++;
            }
        }
    }
}