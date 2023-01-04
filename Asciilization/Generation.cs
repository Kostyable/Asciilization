using System.IO.Pipes;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;

namespace Asciilization;

public static class Generation
{
    public static float[,] valueAlitude;
    public static float[,] valueRainfall;
    public static float maxValue;
    public static Random random = new Random();
    public static float sid = random.Next(1, 9999999);
    public static float zoom = 10f;
    public static Perlin altitude = new Perlin();
    public static Perlin rainfall = new Perlin();
    public static Coordinates[] neighbors = new Coordinates[6];
    public static Coordinates[] nextNeighbors = new Coordinates[6];
    public static int delta;
    public static float minValue;
    public static bool isNeighbor;
    public static List<River> rivers = new List<River>();
    public static List<Hex> sources = new List<Hex>();
    public static List<Hex> riverSources = new List<Hex>();
    public static bool isRegenerate;
    public static int reverseValue;
    
    public static void Map(Map map)
    {
        valueAlitude = new float[map.hexes.GetLength(0), map.hexes.GetLength(1)];
        maxValue = 0f;
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
                else if (valueAlitude[y, x] > maxValue - 0.15f)
                {
                    if (valueAlitude[y, x] < maxValue - 0.07f)
                    {
                        if (valueRainfall[y, x] < 0.38f)
                        {
                            map.hexes[y, x].terrain = Terrain.DesertHills;
                        }
                        else
                        {
                            map.hexes[y, x].terrain = Terrain.PlainHills;
                        }
                        sources.Add(map.hexes[y, x]);
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

    public static void Rivers(Map map, int distance, int quality)
    {
        int i;
        int counter = 0;
        int attempts = 0;
        bool flag;
        while(sources.Count > 0 && !isRegenerate)
        {
            i = random.Next(sources.Count);
            flag = AddRiverSources(sources[i], distance);
            if (flag)
            {
                if (attempts == quality)
                {
                    isRegenerate = false;
                    break;
                }
                attempts++;
                continue;
            }
            rivers.Add(new River());
            GenerateRiver(sources[i], rivers[counter], map, distance, quality);
            if (isRegenerate)
            {
                rivers.RemoveAt(rivers.Count - 1);
                isRegenerate = false;
                break;
            }
            DeleteSources();
            counter++;
        }
        SetRivers(map);
    }

    public static void GenerateRiver(Hex hex, River river, Map map, int distance, int quality)
    {
        int attempts = 0;
        bool flag;
        Hex currentHex = hex;
        int direction = 0;
        int lastTurn = 0;
        int index1 = 0;
        int index2 = 0;
        int index3;
        int index4;
        int side = 0;
        while (true)
        {
            if (attempts == quality)
            {
                isRegenerate = true;
                break;
            }
            river.mainHexes.Add(currentHex);
            riverSources.Add(currentHex);
            flag = SetRiverDirection(ref currentHex, river, map, ref direction, ref index1, ref index2, ref lastTurn, distance, quality);
            if (flag)
            {
                if (isRegenerate)
                {
                    break;
                }
                attempts++;
                continue;
            }
            index3 = index1 + 1;
            if (index3 == 6)
            {
                index3 = 0;
            }
            index4 = index2 - 1;
            if (index4 == -1)
            {
                index4 = 5;
            }
            if (river.mainHexes.Count == 1)
            {
                side = GetStartIndex(index1, index2, index3, index4, map);
                if (side == index1 || side == index3)
                {
                    side = 0;
                }
                else
                {
                    side = 1;
                }
            }
            if (side == 0)
            {
                AddAdditionalHexes(currentHex, river, map, index3);
                AddAdditionalHexes(currentHex, river, map, index1);
            }
            else
            {
                AddAdditionalHexes(currentHex, river, map, index4);
                AddAdditionalHexes(currentHex, river, map, index2);
            }

            SetReverseDirection(currentHex, river, map, direction);
            if (map.hexes[neighbors[direction].y, neighbors[direction].x].terrain == Terrain.Water)
            {
                return;
            }
            currentHex = map.hexes[neighbors[direction].y, neighbors[direction].x];
        }
    }

    public static void SetRivers(Map map)
    {
        foreach (River river in rivers)
        {
            foreach (Hex hex in map.hexes)
            {
                if (river.mainHexes.Contains(hex) || river.additionalHexes.Contains(hex))
                {
                    hex.withRiver = true;
                }

                if (river.directions.ContainsKey(hex))
                {
                    hex.riverDir = river.directions[hex];
                }
            }
        }
    }

    public static int GetStartIndex(int index1, int index2, int index3, int index4, Map map)
    {
        float max = 0f;
        int maxIndex = index1;
        int[] indexes = new int[4];
        indexes[0] = index1;
        indexes[1] = index2;
        indexes[2] = index3;
        indexes[3] = index4;
        for (int i = 0; i < indexes.Length; i++)
        {
            if (valueAlitude[neighbors[indexes[i]].y, neighbors[indexes[i]].x] > max && map.hexes[neighbors[indexes[i]].y, neighbors[indexes[i]].x].terrain != Terrain.Mountains)
            {
                max = valueAlitude[neighbors[indexes[i]].y, neighbors[indexes[i]].x];
                maxIndex = indexes[i];
            }
        }
        return maxIndex;
    }

    public static int GetReverseValue(int value)
    {
        if (value < 3)
        {
            return value + 3;
        }
        return value - 3;
    }

    public static void AddAdditionalHexes(Hex hex, River river, Map map, int value)
    {
        reverseValue = GetReverseValue(value);
        if (river.additionalHexes.Contains(map.hexes[neighbors[value].y, neighbors[value].x]))
        {
            if (river.directions.ContainsKey(hex))
            {
                return;
            }
            river.directions.Add(hex, value);
        }
        else if (map.hexes[neighbors[value].y, neighbors[value].x].terrain != Terrain.Water && map.hexes[neighbors[value].y, neighbors[value].x].terrain != Terrain.Mountains && !river.mainHexes.Contains(map.hexes[neighbors[value].y, neighbors[value].x]))
        {
            river.additionalHexes.Add(map.hexes[neighbors[value].y, neighbors[value].x]);
            river.directions.Add(map.hexes[neighbors[value].y, neighbors[value].x], reverseValue);
        }
    }

    public static void SetReverseDirection(Hex hex, River river, Map map, int value)
    {
        reverseValue = GetReverseValue(value);
        if (river.additionalHexes.Contains(map.hexes[neighbors[reverseValue].y, neighbors[reverseValue].x]))
        {
            if (river.directions.ContainsKey(hex))
            {
                return;
            }
            river.directions.Add(hex, reverseValue);
        }
    }

    public static void InitNeighbors(Hex hex, Coordinates[] arr, Map map)
    {
        if (sources.Contains(hex))
        {
            riverSources.Add(hex);
        }
        if (hex.coordinates.x % 2 == 0)
        {
            delta = 0;
        }
        else
        {
            delta = 1;
        }
        arr[0] = new Coordinates(hex.coordinates.x, hex.coordinates.y - 1);
        arr[1] = new Coordinates(hex.coordinates.x - 1, hex.coordinates.y - 1 + delta);
        arr[2] = new Coordinates(hex.coordinates.x - 1, hex.coordinates.y + delta);
        arr[3] = new Coordinates(hex.coordinates.x, hex.coordinates.y + 1);
        arr[4] = new Coordinates(hex.coordinates.x + 1, hex.coordinates.y + delta);
        arr[5] = new Coordinates(hex.coordinates.x + 1, hex.coordinates.y - 1 + delta);
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].x != -1 && arr[i].x != map.hexes.GetLength(1) && arr[i].y != -1 && arr[i].y != map.hexes.GetLength(0))
            {
                if (sources.Contains(map.hexes[arr[i].y, arr[i].x]))
                {
                    riverSources.Add(map.hexes[arr[i].y, arr[i].x]);
                }
            }
        }
    }
    
    public static bool CheckRiverNeighbors(Hex hex, River river, Map map, ref bool isWater)
    {
        InitNeighbors(hex, nextNeighbors, map);
        bool flag;
        for (int i = 0; i < nextNeighbors.Length; i++)
        {
            if (nextNeighbors[i].x != -1 && nextNeighbors[i].x != map.hexes.GetLength(1) && nextNeighbors[i].y != -1 && nextNeighbors[i].y != map.hexes.GetLength(0))
            {
                if (map.hexes[nextNeighbors[i].y, nextNeighbors[i].x].terrain == Terrain.Water)
                {
                    isWater = true;
                }
                flag = FixCollisions(map.hexes[nextNeighbors[i].y, nextNeighbors[i].x], river);
                if (flag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool SetRiverDirection(ref Hex hex, River river, Map map, ref int direction, ref int index1, ref int index2, ref int lastTurn, int distance, int quality)
    {
        InitNeighbors(hex, neighbors, map);
        int turn = 0;
        minValue = 1f;
        int index = 0;
        int isCircle = 0;
        bool flag;
        int attempts = 0;
        int j;
        bool isWater = false;
        for (int i = 0; i < neighbors.Length; i++)
        {
            isNeighbor = CheckRiverNeighbors(map.hexes[neighbors[i].y, neighbors[i].x], river, map, ref isWater);
            isCircle += FixCircle(map.hexes[neighbors[i].y, neighbors[i].x], river);
            if (isNeighbor || isCircle == 2)
            {
                riverSources.Clear();
                river.Clear();
                while (true)
                {
                    j = random.Next(sources.Count);
                    flag = AddRiverSources(sources[j], distance);
                    if (flag)
                    {
                        if (attempts == quality)
                        {
                            isRegenerate = true;
                            return true;
                        }
                        attempts++;
                    }
                    else
                    {
                        break;
                    }
                }
                hex = sources[j];
                lastTurn = 0;
                direction = 0;
                index1 = 0;
                index2 = 0;
                return true;
            }
        }
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (valueAlitude[neighbors[i].y, neighbors[i].x] < minValue && (index1 == index2 || i == direction || i == index1 || i == index2))
            {
                if (i == direction)
                {
                    turn = 0;
                }
                else if (i == index1)
                {
                    turn = 1;
                }
                else if (i == index2)
                {
                    turn = 2;
                }
                if (index1 == index2 || (isWater && turn == 0) || turn != lastTurn)
                {
                    minValue = valueAlitude[neighbors[i].y, neighbors[i].x];
                    index = i;
                }
            }
        }
        if (index == direction)
        {
            lastTurn = 0;
        }
        else if (index == index1)
        {
            lastTurn = 1;
        }
        else if (index == index2)
        {
            lastTurn = 2;
        }
        direction = index;
        index1 = direction + 1;
        if (index1 == 6)
        {
            index1 = 0;
        }
        index2 = direction - 1;
        if (index2 == -1)
        {
            index2 = 5;
        }
        return false;
    }

    public static bool FixCollisions(Hex hex, River riv)
    {
        foreach (River river in rivers)
        {
            for (int i = 0; i < river.mainHexes.Count; i++)
            {
                if (hex == river.mainHexes[i] && river != riv)
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    public static int FixCircle(Hex hex, River river)
    {
        for (int i = 0; i < river.mainHexes.Count; i++)
        {
            if (hex == river.mainHexes[i])
            {
                return 1;
            }
        }
        return 0;
    }

    public static void DeleteSources()
    {
        foreach (Hex hex in riverSources)
        {
            if (sources.Contains(hex))
            {
                sources.Remove(hex);
            }
        }
        riverSources.Clear();
    }
    
    public static bool AddRiverSources(Hex hex, int distance)
    {
        for (int i = 0; i < sources.Count; i++)
        {
            if (Math.Abs(sources[i].coordinates.x - hex.coordinates.x) <= distance && Math.Abs(sources[i].coordinates.y - hex.coordinates.y) <= distance && sources[i] != hex)
            {
                riverSources.Add(sources[i]);
            }
        }
        if (riverSources.Count == 0)
        {
            return true;
        }
        return false;
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
            if (map.hexes[randY, randX].terrain != Terrain.Water && map.hexes[randY, randX].terrain != Terrain.Mountains && map.hexes[randY, randX].civ == Civ.Without)
            {
                map.hexes[randY, randX].civ = (Civ)i + 1;
                i++;
            }
        }
    }
}