namespace Asciilization;

public static class Generation
{
    public static Random random;
    public static float[,] valueAlitude;
    public static float[,] valueLatitude;
    public static float[,] valueWoodiness;
    public static float maxValueA;
    public static float maxValueL;
    public static float sid;
    public static float zoom;
    public static Perlin altitude;
    public static Perlin latitude;
    public static Perlin woodiness;
    public static Coord[] neighbors;
    public static Coord[] nextNeighbors;
    public static int delta;
    public static float minValue;
    public static bool isNeighbor;
    public static List<River> rivers;
    public static List<Hex> sources;
    public static List<Hex> riverSources;
    public static bool isRegenerate;
    public static int reverseValue;

    public static void Init()
    {
        random = new Random();
        sid = random.Next(1, 9999999);
        zoom = 10f;
        altitude = new Perlin();
        latitude = new Perlin();
        woodiness = new Perlin();
        neighbors = new Coord[6];
        nextNeighbors = new Coord[6];
        rivers = new List<River>();
        sources = new List<Hex>();
        riverSources = new List<Hex>();
    }
    
    public static void Map(Map map)
    {
        Init();
        valueAlitude = new float[map.hexes.GetLength(0), map.hexes.GetLength(1)];
        maxValueA = 0f;
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
                if (valueAlitude[y, x] > maxValueA)
                {
                    maxValueA = valueAlitude[y, x];
                }
            }
        }
        sid = random.Next(1, 9999999);
        valueLatitude = new float[map.hexes.GetLength(0), map.hexes.GetLength(1)];
        maxValueL = 0f;
        for (int y = 0; y < map.hexes.GetLength(0); y++)
        {
            for (int x = 0; x < map.hexes.GetLength(1); x++)
            {
                float nx = (x + sid) / zoom + 0.5f, ny = (y + sid) / zoom + 0.5f;
                valueLatitude[y, x] = (latitude.Noise(nx, ny) + 1f) / 2f - (float)Latitude(map, y);
                if (valueLatitude[y, x] < 0f)
                {
                    valueLatitude[y, x] = 0f;
                }
                if (valueLatitude[y, x] > 1f)
                {
                    valueLatitude[y, x] = 1f;
                }
                if (valueLatitude[y, x] > maxValueL)
                {
                    maxValueL = valueLatitude[y, x];
                }
            }
        }
        sid = random.Next(1, 9999999);
        valueWoodiness = new float[map.hexes.GetLength(0), map.hexes.GetLength(1)];
        for (int y = 0; y < map.hexes.GetLength(0); y++)
        {
            for (int x = 0; x < map.hexes.GetLength(1); x++)
            {
                float nx = (x + sid) / zoom + 0.5f, ny = (y + sid) / zoom + 0.5f;
                valueWoodiness[y, x] = (woodiness.Noise(nx, ny) + 1f) / 2f;
                if (valueWoodiness[y, x] < 0f)
                {
                    valueWoodiness[y, x] = 0f;
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
                else if (valueAlitude[y, x] > maxValueA - 0.13f)
                {
                    if (valueAlitude[y, x] < maxValueA - 0.07f)
                    {
                        if (valueLatitude[y, x] < maxValueL - 0.25f)
                        {
                            map.hexes[y, x].terrain = Terrain.PlainHills;
                        }
                        else
                        {
                            map.hexes[y, x].terrain = Terrain.DesertHills;
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
                    if (valueLatitude[y, x] < maxValueL - 0.25f)
                    {
                        if (valueWoodiness[y, x] >= 0.57f && valueLatitude[y, x] < maxValueL - 0.4f)
                        {
                            map.hexes[y, x].terrain = Terrain.Forest;
                        }
                        else
                        {
                            map.hexes[y, x].terrain = Terrain.Plain;
                        }
                    }
                    else
                    {
                        map.hexes[y, x].terrain = Terrain.Desert;
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
    
    public static double Latitude(Map map, int y)
    {
        double currentY = Math.Abs(y - map.hexes.GetLength(0) / 2 + 1) / (double)(map.hexes.GetLength(0) / 2);
        return currentY;
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
                lastTurn = 0;
                direction = 0;
                index1 = 0;
                index2 = 0;
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
                flag = AddAdditionalHexes(ref currentHex, river, map, index3, index1, distance, quality);
            }
            else
            {
                flag = AddAdditionalHexes(ref currentHex, river, map, index4, index2, distance, quality);
            }
            if (flag)
            {
                if (isRegenerate)
                {
                    break;
                }
                attempts++;
                lastTurn = 0;
                direction = 0;
                index1 = 0;
                index2 = 0;
                continue;
            }
            flag = SetReverseDirection(ref currentHex, river, map, direction, distance, quality);
            if (flag)
            {
                if (isRegenerate)
                {
                    break;
                }
                attempts++;
                lastTurn = 0;
                direction = 0;
                index1 = 0;
                index2 = 0;
                continue;
            }
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
    
    public static bool AddAdditionalHexes(ref Hex hex, River river, Map map, int value1, int value2, int distance, int quality)
    {
        int i;
        int attempts = 0;
        bool flag;
        if (river.additionalHexes.Contains(map.hexes[neighbors[value1].y, neighbors[value1].x]))
        {
            river.directions.Add(hex, value1);
        }
        else if (map.hexes[neighbors[value1].y, neighbors[value1].x].terrain != Terrain.Water && map.hexes[neighbors[value1].y, neighbors[value1].x].terrain != Terrain.Mountains && !river.mainHexes.Contains(map.hexes[neighbors[value1].y, neighbors[value1].x]))
        {
            reverseValue = GetReverseValue(value1);
            river.additionalHexes.Add(map.hexes[neighbors[value1].y, neighbors[value1].x]);
            river.directions.Add(map.hexes[neighbors[value1].y, neighbors[value1].x], reverseValue);
        }
        if (river.additionalHexes.Contains(map.hexes[neighbors[value2].y, neighbors[value2].x]))
        {
            if (river.directions.ContainsKey(hex))
            {
                riverSources.Clear();
                river.Clear();
                while (true)
                {
                    i = random.Next(sources.Count);
                    flag = AddRiverSources(sources[i], distance);
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
                        hex = sources[i];
                        return true;
                    }
                }
            }
            river.directions.Add(hex, value2);
        }
        else if (map.hexes[neighbors[value2].y, neighbors[value2].x].terrain != Terrain.Water && map.hexes[neighbors[value2].y, neighbors[value2].x].terrain != Terrain.Mountains && !river.mainHexes.Contains(map.hexes[neighbors[value2].y, neighbors[value2].x]))
        {
            reverseValue = GetReverseValue(value2);
            river.additionalHexes.Add(map.hexes[neighbors[value2].y, neighbors[value2].x]);
            river.directions.Add(map.hexes[neighbors[value2].y, neighbors[value2].x], reverseValue);
        }
        return false;
    }
    
    public static bool SetReverseDirection(ref Hex hex, River river, Map map, int value, int distance, int quality)
    {
        int i;
        int attempts = 0;
        bool flag;
        reverseValue = GetReverseValue(value);
        if (river.additionalHexes.Contains(map.hexes[neighbors[reverseValue].y, neighbors[reverseValue].x]))
        {
            if (river.directions.ContainsKey(hex))
            {
                riverSources.Clear();
                river.Clear();
                while (true)
                {
                    i = random.Next(sources.Count);
                    flag = AddRiverSources(sources[i], distance);
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
                        hex = sources[i];
                        return true;
                    }
                }
            }
            river.directions.Add(hex, reverseValue);
        }
        return false;
    }
    
    public static void InitNeighbors(Hex hex, Coord[] arr, Map map)
    {
        if (sources.Contains(hex))
        {
            riverSources.Add(hex);
        }
        if (hex.coord.x % 2 == 0)
        {
            delta = 0;
        }
        else
        {
            delta = 1;
        }
        arr[0] = new Coord(hex.coord.x, hex.coord.y - 1);
        arr[1] = new Coord(hex.coord.x - 1, hex.coord.y - 1 + delta);
        arr[2] = new Coord(hex.coord.x - 1, hex.coord.y + delta);
        arr[3] = new Coord(hex.coord.x, hex.coord.y + 1);
        arr[4] = new Coord(hex.coord.x + 1, hex.coord.y + delta);
        arr[5] = new Coord(hex.coord.x + 1, hex.coord.y - 1 + delta);
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
                        hex = sources[j];
                        return true;
                    }
                }
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
            if (Math.Abs(sources[i].coord.x - hex.coord.x) <= distance && Math.Abs(sources[i].coord.y - hex.coord.y) <= distance && sources[i] != hex)
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
    
    public static void Settlers(Map map, Civilization[] civilizations)
    {
        int i = 0;
        Coord randomCoord;
        bool nearWater;
        bool neighborCiv;
        Neighbors neighbors;
        while (i < civilizations.Length)
        {
            randomCoord.x = random.Next(map.hexes.GetLength(1));
            randomCoord.y = random.Next(map.hexes.GetLength(0));
            if (map.hexes[randomCoord.y, randomCoord.x].terrain != Terrain.Water && map.hexes[randomCoord.y, randomCoord.x].terrain != Terrain.Mountains)
            {
                nearWater = false;
                neighborCiv = false;
                neighbors = map.hexes[randomCoord.y, randomCoord.x].neighbors;
                for (int j = 0; j < neighbors.hexes.Length; j++)
                {
                    if (neighbors.hexes[j] != null)
                    {
                        for (int k = 0; k < i; k++)
                        {
                            if (map.hexes[randomCoord.y, randomCoord.x] == civilizations[k].units[0].currentHex || neighbors.hexes[j] == civilizations[k].units[0].currentHex)
                            {
                                neighborCiv = true;
                                break;
                            }
                        }
                        if (neighborCiv)
                        {
                            break;
                        }
                        if (sources.Contains(map.hexes[randomCoord.y, randomCoord.x]) || neighbors.hexes[j].terrain == Terrain.Water)
                        {
                            nearWater = true;
                        }
                    }
                }
                if ((map.hexes[randomCoord.y, randomCoord.x].withRiver || nearWater) && !neighborCiv)
                {
                    civilizations[i].units.Add(new Settler(map.hexes[randomCoord.y, randomCoord.x], civilizations[i]));
                    i++;
                }
            }
        }
    }
}