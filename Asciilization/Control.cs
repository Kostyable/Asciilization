namespace Asciilization;

public class Control
{
    public static ConsoleKeyInfo input;
    public static bool isGrid;
    
    public static void Map(Map map)
    {
        while (true)
        {
            Move(map);
            if (input.Key == ConsoleKey.LeftArrow && map.isSelected.coord.x > 0)
            {
                OffsetLeft(map);
            }
            else if (input.Key == ConsoleKey.RightArrow && map.isSelected.coord.x < map.hexes.GetLength(1) - 1)
            {
                OffsetRight(map);
            }
            else if (input.Key == ConsoleKey.UpArrow && map.isSelected.coord.y > 0)
            {
                OffsetUp(map);
            }
            else if (input.Key == ConsoleKey.DownArrow && map.isSelected.coord.y < map.hexes.GetLength(0) - 1)
            {
                OffsetDown(map);
            }
            else if (input.Key == ConsoleKey.Z && Printing.scale < 8)
            {
                ZoomIn(map);
            }
            else if (input.Key == ConsoleKey.X && Printing.scale > 0)
            {
                ZoomOut(map);
            }
            else if (input.Key == ConsoleKey.R)
            {
                Regenerate(map);
            }
            else if (input.Key == ConsoleKey.G)
            {
                isGrid = !isGrid;
                Grid(map);
            }
            else if (input.Key == ConsoleKey.Escape)
            {
                Console.SetCursorPosition(Console.WindowWidth - 1, Console.WindowHeight - 1);
                break;
            }
        }
    }
    
    public static void Move(Map map)
    {
        Printing.Cursor(map.isSelected);
        input = Console.ReadKey();
        Printing.NotCursor(map.isSelected);
        if (isGrid)
        {
            Printing.Grid(map.isSelected);
        }
    }
    
    public static void OffsetLeft(Map map)
    {
        if (map.isSelected.coord.x - Printing.offset.x == 0)
        {
            Printing.offset.x--;
            Console.Clear();
            Printing.Map(map);
            if (isGrid)
            {
                Grid(map);
            }
        }
        if (map.isSelected.coord.y - Printing.offset.y == Printing.screenSize.y && map.isSelected.coord.x % 2 == 0)
        {
            map.isSelected = map.hexes[map.isSelected.coord.y - 1, map.isSelected.coord.x - 1];
        }
        else
        {
            map.isSelected = map.hexes[map.isSelected.coord.y, map.isSelected.coord.x - 1];
        }
    }
    
    public static void OffsetRight(Map map)
    {
        if (Printing.scale == 0)
        {
            if (map.isSelected.coord.x - Printing.offset.x == Printing.screenSize.x - 1 && (Printing.hexSize.x - Printing.scale) * (Console.WindowWidth / (Printing.hexSize.x - Printing.scale)) + Printing.scale < Console.WindowWidth || map.isSelected.coord.x - Printing.offset.x == Printing.screenSize.x)
            {
                Printing.offset.x++;
                Console.Clear();
                Printing.Map(map);
                if (isGrid)
                {
                    Grid(map);
                }
            }
        }
        else
        {
            if (map.isSelected.coord.x - Printing.offset.x == Printing.screenSize.x - 1)
            {
                Printing.offset.x++;
                Console.Clear();
                Printing.Map(map);
                if (isGrid)
                {
                    Grid(map);
                }
            }
        }
        if (map.isSelected.coord.y - Printing.offset.y == Printing.screenSize.y && map.isSelected.coord.x % 2 == 0)
        {
            map.isSelected = map.hexes[map.isSelected.coord.y - 1, map.isSelected.coord.x + 1];
        }
        else
        {
            map.isSelected = map.hexes[map.isSelected.coord.y, map.isSelected.coord.x + 1];
        }
    }
    
    public static void OffsetUp(Map map)
    {
        if (map.isSelected.coord.y - Printing.offset.y == 0)
        {
            Printing.offset.y--;
            Console.Clear();
            Printing.Map(map);
            if (isGrid)
            {
                Grid(map);
            }
        }
        map.isSelected = map.hexes[map.isSelected.coord.y - 1, map.isSelected.coord.x];
    }
    
    public static void OffsetDown(Map map)
    {
        if (Console.WindowHeight - Printing.hexSize.y * Printing.screenSize.y < Printing.hexSize.y)
        {
            if (map.isSelected.coord.y - Printing.offset.y == Printing.screenSize.y - 1)
            {
                Printing.offset.y++;
                Console.Clear();
                Printing.Map(map);
                if (isGrid)
                {
                    Grid(map);
                }
            }
        }
        else
        {
            if (map.isSelected.coord.y != map.hexes.GetLength(0) - 1 && (map.isSelected.coord.y - Printing.offset.y == Printing.screenSize.y - 1 && map.isSelected.coord.x % 2 != 0 || map.isSelected.coord.y - Printing.offset.y == Printing.screenSize.y && map.isSelected.coord.x % 2 == 0))
            {
                Printing.offset.y++;
                Console.Clear();
                Printing.Map(map);
                if (isGrid)
                {
                    Grid(map);
                }
            }
        }
        map.isSelected = map.hexes[map.isSelected.coord.y + 1, map.isSelected.coord.x];
    }
    
    public static void ZoomIn(Map map)
    {
        Printing.Init(Printing.hexSize.x + 4, Printing.hexSize.y + 2, Printing.scale + 1);
        CursorInCenter(map);
        Console.Clear();
        Printing.Map(map);
        if (isGrid)
        {
            Grid(map);
        }
    }
    
    public static void ZoomOut(Map map)
    {
        Printing.Init(Printing.hexSize.x - 4, Printing.hexSize.y - 2, Printing.scale - 1);
        CursorInCenter(map);
        if (Printing.scale == 0 && Printing.offset.x == map.hexes.GetLength(1) - Printing.screenSize.x)
        {
            Printing.offset.x--;
        }
        Console.Clear();
        Printing.Map(map);
        if (isGrid)
        {
            Grid(map);
        }
    }
    
    public static void Regenerate(Map map)
    {
        Console.Clear();
        foreach (Hex hex in map.hexes)
        {
            hex.civ = Civ.Without;
            hex.withRiver = false;
            hex.riverDir = 6;
        }
        Generation.sources.Clear();
        Generation.riverSources.Clear();
        Generation.rivers.Clear();
        Console.Write("\x1b[48;2;" + 0 + ";" + 0 + ";" + 0 + "m");
        Game.Launch(map);
        if (isGrid)
        {
            Grid(map);
        }
    }
    
    public static void CursorInCenter(Map map)
    {
        if (map.isSelected.coord.x <= Printing.screenSize.x / 2 - 1)
        {
            Printing.offset.x = 0;
        }
        else if (map.isSelected.coord.x >= map.hexes.GetLength(1) - Printing.screenSize.x / 2 - 1)
        {
            Printing.offset.x = map.hexes.GetLength(1) - Printing.screenSize.x;
        }
        else
        {
            if (Printing.screenSize.x % 2 == 0)
            {
                Printing.offset.x = map.isSelected.coord.x - Printing.screenSize.x / 2 + 1;
            }
            else
            {
                Printing.offset.x = map.isSelected.coord.x - Printing.screenSize.x / 2;
            }
        }
        if (map.isSelected.coord.y <= Printing.screenSize.y / 2 - 1)
        {
            Printing.offset.y = 0;
        }
        else if (map.isSelected.coord.y >= map.hexes.GetLength(0) - Printing.screenSize.y / 2 - 1)
        {
            Printing.offset.y = map.hexes.GetLength(0) - Printing.screenSize.y;
        }
        else
        {
            if (Printing.screenSize.y % 2 == 0)
            {
                Printing.offset.y = map.isSelected.coord.y - Printing.screenSize.y / 2 + 1;
            }
            else
            {
                Printing.offset.y = map.isSelected.coord.y - Printing.screenSize.y / 2;
            }
        }
    }
    
    public static void Grid(Map map)
    {
        if (isGrid)
        {
            for (int i = Printing.offset.y; i <= Printing.screenSize.y + Printing.offset.y; i++)
            {
                for (int j = Printing.offset.x; j < Printing.screenSize.x + Printing.offset.x; j++)
                {
                    if (i < Printing.screenSize.y + Printing.offset.y || (i == Printing.screenSize.y + Printing.offset.y && j % 2 == 0 && Console.WindowHeight - Printing.hexSize.y * Printing.screenSize.y >= Printing.hexSize.y && i != map.hexes.GetLength(0)))
                    {
                        Printing.Grid(map.hexes[i, j]);
                    }
                }
                if (Printing.scale == 0 && i != Printing.screenSize.y + Printing.offset.y && Printing.hexSize.x * Printing.screenSize.x + Printing.hexSize.x == Console.WindowWidth)
                {
                    Printing.Grid(map.hexes[i, Printing.screenSize.x + Printing.offset.x]);
                }
            }
        }
        else
        {
            for (int i = Printing.offset.y; i <= Printing.screenSize.y + Printing.offset.y; i++)
            {
                for (int j = Printing.offset.x; j < Printing.screenSize.x + Printing.offset.x; j++)
                {
                    if (i < Printing.screenSize.y + Printing.offset.y || (i == Printing.screenSize.y + Printing.offset.y && j % 2 == 0 && Console.WindowHeight - Printing.hexSize.y * Printing.screenSize.y >= Printing.hexSize.y && i != map.hexes.GetLength(0)))
                    {
                        Printing.NotGrid(map.hexes[i, j]);
                    }
                }
                if (Printing.scale == 0 && i != Printing.screenSize.y + Printing.offset.y && Printing.hexSize.x * Printing.screenSize.x + Printing.hexSize.x == Console.WindowWidth)
                {
                    Printing.NotGrid(map.hexes[i, Printing.screenSize.x + Printing.offset.x]);
                }
            }
        }
    }
}