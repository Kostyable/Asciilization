namespace Asciilization;

public class Control
{
    public static ConsoleKeyInfo input;
    public static bool isSelectMode;
    public static bool isGrid;
    public static Unit? selectedUnit;
    public static bool flag;
    
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
            else if (input.Key == ConsoleKey.Z && Output.scale < 8)
            {
                ZoomIn(map);
            }
            else if (input.Key == ConsoleKey.X && Output.scale > 0)
            {
                ZoomOut(map);
            }
            else if (input.Key == ConsoleKey.G)
            {
                isGrid = !isGrid;
                Grid(map);
            }
            else if (input.Key == ConsoleKey.B && isSelectMode)
            {
                ((Settler)Game.playerCiv.units[0]).BuildCity();
                isSelectMode = false;
                Game.playerCiv.units.Clear();
                Rewrite(map);
            }
            else if (input.Key == ConsoleKey.Enter)
            {
                flag = false;
                if (isSelectMode && selectedUnit != null && selectedUnit.currentHex.neighbors.hexes.Contains(map.isSelected) && map.isSelected.terrain != Terrain.Water && map.isSelected.terrain != Terrain.Mountains)
                {
                    for (int i = 0; i < Game.civilizations.Length; i++)
                    {
                        if (selectedUnit.currentHex.neighbors.hexes.Contains(Game.civilizations[i].units[0].currentHex) && map.isSelected == Game.civilizations[i].units[0].currentHex)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        selectedUnit.Move(map.isSelected);
                        Rewrite(map);
                    }
                }
                isSelectMode = !isSelectMode;
                if (Game.playerCiv.units.Count > 0 && Game.playerCiv.units[0].currentHex == map.isSelected && isSelectMode)
                {
                    selectedUnit = Game.playerCiv.units[0];
                }
                else
                {
                    selectedUnit = null;
                }
            }
            else if (input.Key == ConsoleKey.R)
            {
                Regenerate(map);
            }
            else if (input.Key == ConsoleKey.Escape)
            {
                Console.SetCursorPosition(Console.WindowWidth - 1, Console.WindowHeight - 1);
                break;
            }
        }
    }

    public static void Rewrite(Map map)
    {
        Output.hexesLayer.Clear();
        Output.riversLayer.Clear();
        Output.uiLayer.Clear();
        Console.SetCursorPosition(0, 0);
        Output.Map(map);
        if (isGrid)
        {
            Grid(map);
        }
    }
    
    public static void Move(Map map)
    {
        if (isSelectMode)
        {
            Output.Cursor(map.isSelected, map, "145");
        }
        else
        {
            Output.Cursor(map.isSelected, map, "231");
        }
        input = Console.ReadKey(true);
        Output.NotCursor(map.isSelected, map);
        if (isGrid)
        {
            Output.Grid(map.isSelected, map);
        }
    }
    
    public static void OffsetLeft(Map map)
    {
        if (map.isSelected.coord.x - Output.offset.x == 0)
        {
            Output.offset.x--;
            Rewrite(map);
        }
        if (map.isSelected.coord.y - Output.offset.y == Output.screenSize.y && map.isSelected.coord.x % 2 == 0)
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
        if (map.isSelected.coord.x - Output.offset.x == Output.screenSize.x - 1)
        {
            Output.offset.x++;
            Rewrite(map);
        }
        if (map.isSelected.coord.y - Output.offset.y == Output.screenSize.y && map.isSelected.coord.x % 2 == 0)
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
        if (map.isSelected.coord.y - Output.offset.y == 0)
        {
            Output.offset.y--;
            Rewrite(map);
        }
        map.isSelected = map.hexes[map.isSelected.coord.y - 1, map.isSelected.coord.x];
    }
    
    public static void OffsetDown(Map map)
    {
        if (Console.WindowHeight - Output.hexSize.y * Output.screenSize.y < Output.hexSize.y)
        {
            if (map.isSelected.coord.y - Output.offset.y == Output.screenSize.y - 1)
            {
                Output.offset.y++;
                Rewrite(map);
            }
        }
        else
        {
            if (map.isSelected.coord.y != map.hexes.GetLength(0) - 1 && (map.isSelected.coord.y - Output.offset.y == Output.screenSize.y - 1 && map.isSelected.coord.x % 2 != 0 || map.isSelected.coord.y - Output.offset.y == Output.screenSize.y && map.isSelected.coord.x % 2 == 0))
            {
                Output.offset.y++;
                Rewrite(map);
            }
        }
        map.isSelected = map.hexes[map.isSelected.coord.y + 1, map.isSelected.coord.x];
    }
    
    public static void ZoomIn(Map map)
    {
        Output.Init(Output.hexSize.x + 4, Output.hexSize.y + 2, Output.scale + 1);
        CursorInCenter(map);
        Rewrite(map);
    }
    
    public static void ZoomOut(Map map)
    {
        Output.Init(Output.hexSize.x - 4, Output.hexSize.y - 2, Output.scale - 1);
        CursorInCenter(map);
        Rewrite(map);
    }
    
    public static void Regenerate(Map map)
    {
        isSelectMode = false;
        foreach (Hex hex in map.hexes)
        {
            hex.civColor = CivColors.Without;
            hex.withRiver = false;
            hex.riverDir = 6;
        }
        Generation.sources.Clear();
        Generation.riverSources.Clear();
        Generation.rivers.Clear();
        foreach (Civilization civ in Game.civilizations)
        {
            civ.units.Clear();
        }
        Output.hexesLayer.Clear();
        Output.riversLayer.Clear();
        Output.uiLayer.Clear();
        Console.SetCursorPosition(0, 0);
        Game.playerCiv = Game.civilizations[Generation.random.Next(Enum.GetNames(typeof(CivNames)).Length)];
        Game.Launch(map, Game.civilizations);
        if (isGrid)
        {
            Grid(map);
        }
    }
    
    public static void CursorInCenter(Map map)
    {
        if (map.isSelected.coord.x <= Output.screenSize.x / 2 - 1)
        {
            Output.offset.x = 0;
        }
        else if (map.isSelected.coord.x >= map.hexes.GetLength(1) - Output.screenSize.x / 2 - 1)
        {
            Output.offset.x = map.hexes.GetLength(1) - Output.screenSize.x;
        }
        else
        {
            if (Output.screenSize.x % 2 == 0)
            {
                Output.offset.x = map.isSelected.coord.x - Output.screenSize.x / 2 + 1;
            }
            else
            {
                Output.offset.x = map.isSelected.coord.x - Output.screenSize.x / 2;
            }
        }
        if (map.isSelected.coord.y <= Output.screenSize.y / 2 - 1)
        {
            Output.offset.y = 0;
        }
        else if (map.isSelected.coord.y >= map.hexes.GetLength(0) - Output.screenSize.y / 2 - 1)
        {
            Output.offset.y = map.hexes.GetLength(0) - Output.screenSize.y;
        }
        else
        {
            if (Output.screenSize.y % 2 == 0)
            {
                Output.offset.y = map.isSelected.coord.y - Output.screenSize.y / 2 + 1;
            }
            else
            {
                Output.offset.y = map.isSelected.coord.y - Output.screenSize.y / 2;
            }
        }
    }
    
    public static void Grid(Map map)
    {
        Output.gridLayer.Clear();
        if (isGrid)
        {
            for (int i = Output.offset.y; i <= Output.screenSize.y + Output.offset.y; i++)
            {
                for (int j = Output.offset.x; j < Output.screenSize.x + Output.offset.x; j++)
                {
                    if (i < Output.screenSize.y + Output.offset.y || (i == Output.screenSize.y + Output.offset.y && j % 2 == 0 && Console.WindowHeight - Output.hexSize.y * Output.screenSize.y >= Output.hexSize.y && i != map.hexes.GetLength(0)))
                    {
                        Output.SbGrid(map.hexes[i, j], map);
                    }
                }
                if (Output.scale == 0 && i != Output.screenSize.y + Output.offset.y && Output.hexSize.x * Output.screenSize.x + Output.hexSize.x == Console.WindowWidth)
                {
                    Output.SbGrid(map.hexes[i, Output.screenSize.x + Output.offset.x], map);
                }
            }
        }
        else
        {
            for (int i = Output.offset.y; i <= Output.screenSize.y + Output.offset.y; i++)
            {
                for (int j = Output.offset.x; j < Output.screenSize.x + Output.offset.x; j++)
                {
                    if (i < Output.screenSize.y + Output.offset.y || (i == Output.screenSize.y + Output.offset.y && j % 2 == 0 && Console.WindowHeight - Output.hexSize.y * Output.screenSize.y >= Output.hexSize.y && i != map.hexes.GetLength(0)))
                    {
                        Output.SbNotGrid(map.hexes[i, j], map);
                    }
                }
                if (Output.scale == 0 && i != Output.screenSize.y + Output.offset.y && Output.hexSize.x * Output.screenSize.x + Output.hexSize.x == Console.WindowWidth)
                {
                    Output.SbNotGrid(map.hexes[i, Output.screenSize.x + Output.offset.x], map);
                }
            }
        }
        Console.SetCursorPosition(0, 0);
        Console.Write(Output.gridLayer);
    }
}