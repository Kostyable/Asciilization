namespace Asciilization;

public class Printing
{
    public static Coordinates hexSize;
    public static Coordinates screenSize;
    public static Coordinates offset;
    public static int scale;
    public static Coordinates cursor;
    public static int k;

    public static void Init(int hexSizeX, int hexSizeY, int offsetX, int offsetY, int sc)
    {
        hexSize = new Coordinates(hexSizeX, hexSizeY);
        offset = new Coordinates(offsetX, offsetY);
        scale = sc;
        SetScreenSize();
    }
    
    public static void Init(int hexSizeX, int hexSizeY, int sc)
    {
        hexSize = new Coordinates(hexSizeX, hexSizeY);
        scale = sc;
        SetScreenSize();
    } 

    public static void Map(Map map)
    {
        Console.SetCursorPosition(0, 0);
        Console.Write($"{screenSize.x} {screenSize.y}");
        if (offset.x == 0 || offset.y == 0)
        {
            FantomHex(offset.x - 1, offset.y - 1);
        }
        else
        {
            HalfHex(map.hexes[offset.y - 1, offset.x - 1]);
        }
        for (int j = offset.x; j < screenSize.x + offset.x; j++)
        {
            if (offset.y == 0)
            {
                FantomHex(j, -1);
            }
            else
            {
                HalfHex(map.hexes[offset.y - 1, j]);
            }
        }
        if (offset.x == map.hexes.GetLength(1) - screenSize.x || offset.y == 0)
        {
            FantomHex(screenSize.x + offset.x, offset.y - 1);
        }
        else
        {
            HalfHex(map.hexes[offset.y - 1, screenSize.x + offset.x]);
        }
        for (int i = offset.y; i < screenSize.y + offset.y; i++)
        {
            if (offset.x == 0)
            {
                FantomHex(-1, i);
            }
            else
            {
                HalfHex(map.hexes[i, offset.x - 1]);
            }
            for (int j = offset.x; j < screenSize.x + offset.x; j++)
            {
                Hex(map.hexes[i, j]);
            }
            if (offset.x == map.hexes.GetLength(1) - screenSize.x)
            {
                FantomHex(map.hexes.GetLength(1), i);
            }
            else
            {
                HalfHex(map.hexes[i, screenSize.x + offset.x]);
            }
        }
        if (offset.x == 0 || offset.y == map.hexes.GetLength(0) - screenSize.y)
        {
            FantomHex(offset.x - 1, screenSize.y + offset.y);
            FantomHex(offset.x - 1, screenSize.y + offset.y + 1);
        }
        else if (offset.y == map.hexes.GetLength(0) - screenSize.y - 1)
        {
            HalfHex(map.hexes[screenSize.y + offset.y, offset.x - 1]);
            FantomHex(offset.x - 1, screenSize.y + offset.y + 1);
        }
        else
        {
            HalfHex(map.hexes[screenSize.y + offset.y, offset.x - 1]);
            HalfHex(map.hexes[screenSize.y + offset.y + 1, offset.x - 1]);
        }
        for (int j = offset.x; j < screenSize.x + offset.x; j++)
        {
            if (offset.y == map.hexes.GetLength(0) - screenSize.y)
            {
                FantomHex(j, map.hexes.GetLength(0));
                FantomHex(j, map.hexes.GetLength(0) + 1);
            }
            else if (offset.y == map.hexes.GetLength(0) - screenSize.y - 1)
            {
                HalfHex(map.hexes[screenSize.y + offset.y, j]);
                FantomHex(j, map.hexes.GetLength(0));
            }
            else
            {
                HalfHex(map.hexes[screenSize.y + offset.y, j]);
                HalfHex(map.hexes[screenSize.y + offset.y + 1, j]);
            }
        }
        if (offset.x == map.hexes.GetLength(1) - screenSize.x || offset.y == map.hexes.GetLength(0) - screenSize.y)
        {
            FantomHex(screenSize.x + offset.x, screenSize.y + offset.y);
            FantomHex(screenSize.x + offset.x, screenSize.y + offset.y + 1);
        }
        else if (offset.y == map.hexes.GetLength(0) - screenSize.y - 1)
        {
            HalfHex(map.hexes[screenSize.y + offset.y, screenSize.x + offset.x]);
            FantomHex(screenSize.x + offset.x, screenSize.y + offset.y + 1);
        }
        else
        {
            HalfHex(map.hexes[screenSize.y + offset.y, screenSize.x + offset.x]);
            HalfHex(map.hexes[screenSize.y + offset.y + 1, screenSize.x + offset.x]);
        }
        Rivers(map);
    }

    public static void Hex(Hex hex)
    {
        CountCursorPosition(hex.coordinates.x, hex.coordinates.y);
        SetBackgroundColor(hex);
        for (int i = 0; i < hexSize.y / 2; i++)
        {
            k = scale - i;
            Console.SetCursorPosition(cursor.x + k, cursor.y + i);
            for (int j = 0; j < hexSize.x - 2 * k; j += 2)
            {
                PrintChar(hex);
            }
            Console.SetCursorPosition(cursor.x + i, cursor.y + hexSize.y / 2 + i);
            for (int j = 0; j < hexSize.x - 2 * i; j += 2)
            {
                PrintChar(hex);
            }
        }
    }

    public static void HalfHex(Hex hex)
    {
        CountCursorPosition(hex.coordinates.x, hex.coordinates.y);
        SetBackgroundColor(hex);
        int bU = 0;
        int bD = 0;
        int bL = 0;
        int bR = 0;
        int j;
        if (Console.WindowHeight - cursor.y < hexSize.y)
        {
            bD = hexSize.y - (Console.WindowHeight - cursor.y);
        }
        if (cursor.y < 0)
        {
            bU = -cursor.y;
        }
        for (int i = bU; i < hexSize.y - bD; i++)
        {
            j = Math.Abs(scale - i) - i / (hexSize.y / 2);
            if (Console.WindowWidth - cursor.x < hexSize.x)
            {
                bR = hexSize.x - (Console.WindowWidth - cursor.x + j);
            }
            if (cursor.x < 0)
            {
                bL = -(cursor.x + j);
            }
            if (Console.WindowWidth - scale == cursor.x && j == scale)
            {
                continue;
            }
            Console.SetCursorPosition(cursor.x + j + bL, cursor.y + i);
            for (int l = bL; l < hexSize.x - bR - 2 * j; l++)
            {
                if (l % 2 == 0)
                {
                    HalfPrintChar(hex);
                }
                else
                {
                    Console.Write(" ");
                }
            }
        }
    }
    
    public static void FantomHex(int x, int y)
    {
        CountCursorPosition(x, y);
        int bU = 0;
        int bD = 0;
        int bL = 0;
        int bR = 0;
        int j;
        if (Console.WindowHeight - cursor.y < hexSize.y)
        {
            bD = hexSize.y - (Console.WindowHeight - cursor.y);
        }
        if (cursor.y < 0)
        {
            bU = -cursor.y;
        }
        Console.Write("\x1b[38;2;" + 0 + ";" + 0 + ";" + 200 + "m");
        Console.Write("\x1b[48;2;" + 0 + ";" + 0 + ";" + 0 + "m");
        for (int i = bU; i < hexSize.y - bD; i++)
        {
            j = Math.Abs(scale - i) - i / (hexSize.y / 2);
            if (Console.WindowWidth - cursor.x < hexSize.x)
            {
                bR = hexSize.x - (Console.WindowWidth - cursor.x + j);
            }
            if (cursor.x < 0)
            {
                bL = -(cursor.x + j);
            }
            if (Console.WindowWidth - scale == cursor.x && j == scale)
            {
                continue;
            }
            Console.SetCursorPosition(cursor.x + j + bL, cursor.y + i);
            for (int l = bL; l < hexSize.x - bR - 2 * j; l++)
            {
                if (l % 2 == 0)
                {
                    Console.Write("~");
                }
                else
                {
                    Console.Write(" ");
                }
            }
        }
    }

    public static void Rivers(Map map)
    {
        Console.Write("\x1b[38;2;" + 0 + ";" + 0 + ";" + 200 + "m");
        for (int i = offset.y - 1; i < screenSize.y + offset.y + 2; i++)
        {
            for (int j = offset.x - 1; j < screenSize.x + offset.x + 1; j++)
            {
                if (i != -1 && i < map.hexes.GetLength(0) && j != -1 && j != map.hexes.GetLength(1))
                {
                    if (map.hexes[i, j].riverDir != 6 && scale > 0)
                    {
                        River(map.hexes[i, j], map);
                    }
                }
            }
        }
    }

    public static void River(Hex hex, Map map)
    {
        int delta;
        CountCursorPosition(hex.coordinates.x, hex.coordinates.y);
        if (hex.coordinates.x % 2 == 0)
        {
            delta = 0;
        }
        else
        {
            delta = 1;
        }
        if (hex.riverDir == 0)
        {
            for (int i = 0; i < scale + 2; i++)
            {
                if (cursor.x + scale + 2 * i >= 0 && cursor.x + scale + 2 * i < Console.WindowWidth)
                {
                    if (cursor.y >= 0 && cursor.y < Console.WindowHeight)
                    {
                        SetBackgroundColor(hex);
                        Console.SetCursorPosition(cursor.x + scale + 2 * i, cursor.y);
                        Console.Write("~");
                    }
                    if (cursor.y - 1 >= 0 && cursor.y - 1 < Console.WindowHeight)
                    {
                        SetBackgroundColor(map.hexes[hex.coordinates.y - 1, hex.coordinates.x]);
                        Console.SetCursorPosition(cursor.x + scale + 2 * i, cursor.y - 1);
                        Console.Write("~");
                    }
                }
            }
        }
        else if (hex.riverDir == 1)
        {
            for (int i = 0; i < hexSize.y / 2; i++)
            {
                if (cursor.y + i >= 0 && cursor.y + i < Console.WindowHeight)
                {
                    if (cursor.x + scale - i >= 0 && cursor.x + scale - i < Console.WindowWidth)
                    {
                        SetBackgroundColor(hex);
                        Console.SetCursorPosition(cursor.x + scale - i, cursor.y + i);
                        Console.Write("~");
                    }
                    if (cursor.x + scale - i - 2 >= 0 && cursor.x + scale - i - 2 < Console.WindowWidth)
                    {
                        SetBackgroundColor(map.hexes[hex.coordinates.y - 1 + delta, hex.coordinates.x - 1]);
                        Console.SetCursorPosition(cursor.x + scale - i - 2, cursor.y + i);
                        Console.Write("~");
                    }
                }
            }
        }
        else if (hex.riverDir == 2)
        {
            for (int i = 0; i < hexSize.y / 2; i++)
            {
                if (cursor.y + hexSize.y / 2 + i >= 0 && cursor.y + hexSize.y / 2 + i < Console.WindowHeight)
                {
                    if (cursor.x + i >= 0 && cursor.x + i < Console.WindowWidth)
                    {
                        SetBackgroundColor(hex);
                        Console.SetCursorPosition(cursor.x + i, cursor.y + hexSize.y / 2 + i);
                        Console.Write("~");
                    }

                    if (cursor.x + i - 2 >= 0 && cursor.x + i - 2 < Console.WindowWidth)
                    {
                        SetBackgroundColor(map.hexes[hex.coordinates.y + delta, hex.coordinates.x - 1]);
                        Console.SetCursorPosition(cursor.x + i - 2, cursor.y + hexSize.y / 2 + i);
                        Console.Write("~");
                    }
                }
            }
        }
        else if (hex.riverDir == 3)
        {
            for (int i = 0; i < scale + 2; i++)
            {
                if (cursor.x + scale + 2 * i >= 0 && cursor.x + scale + 2 * i < Console.WindowWidth)
                {
                    if (cursor.y + hexSize.y - 1 >= 0 && cursor.y + hexSize.y - 1 < Console.WindowHeight)
                    {
                        SetBackgroundColor(hex);
                        Console.SetCursorPosition(cursor.x + scale + 2 * i, cursor.y + hexSize.y - 1);
                        Console.Write("~");
                    }
                    if (cursor.y + hexSize.y >= 0 && cursor.y + hexSize.y < Console.WindowHeight)
                    {
                        SetBackgroundColor(map.hexes[hex.coordinates.y + 1, hex.coordinates.x]);
                        Console.SetCursorPosition(cursor.x + scale + 2 * i, cursor.y + hexSize.y);
                        Console.Write("~");
                    }
                }
            }
        }
        else if (hex.riverDir == 4)
        {
            for (int i = 0; i < hexSize.y / 2; i++)
            {
                if (cursor.y + hexSize.y - 1 - i >= 0 && cursor.y + hexSize.y - 1 - i < Console.WindowHeight)
                {
                    if (cursor.x + hexSize.x - scale - 2 + i >= 0 && cursor.x + hexSize.x - scale - 2 + i < Console.WindowWidth)
                    {
                        SetBackgroundColor(hex);
                        Console.SetCursorPosition(cursor.x + hexSize.x - scale - 2 + i, cursor.y + hexSize.y - 1 - i);
                        Console.Write("~");
                    }
                    if (cursor.x + hexSize.x - scale + i >= 0 && cursor.x + hexSize.x - scale + i < Console.WindowWidth)
                    {
                        SetBackgroundColor(map.hexes[hex.coordinates.y + delta, hex.coordinates.x + 1]);
                        Console.SetCursorPosition(cursor.x + hexSize.x - scale + i, cursor.y + hexSize.y - 1 - i);
                        Console.Write("~");
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < hexSize.y / 2; i++)
            {
                if (cursor.y + hexSize.y / 2 - 1 - i >= 0 && cursor.y + hexSize.y / 2 - 1 - i < Console.WindowHeight)
                {
                    if (cursor.x + hexSize.x - 2 - i >= 0 && cursor.x + hexSize.x - 2 - i < Console.WindowWidth)
                    {
                        SetBackgroundColor(hex);
                        Console.SetCursorPosition(cursor.x + hexSize.x - 2 - i, cursor.y + hexSize.y / 2 - 1 - i);
                        Console.Write("~");
                    }

                    if (cursor.x + hexSize.x - i >= 0 && cursor.x + hexSize.x - i < Console.WindowWidth)
                    {
                        SetBackgroundColor(map.hexes[hex.coordinates.y - 1 + delta, hex.coordinates.x + 1]);
                        Console.SetCursorPosition(cursor.x + hexSize.x - i, cursor.y + hexSize.y / 2 - 1 - i);
                        Console.Write("~");
                    }
                }
            }
        }
    }

    public static void Cursor(Hex hex)
    {
        CountCursorPosition(hex.coordinates.x, hex.coordinates.y);
        Console.Write("\x1b[38;2;" + 255 + ";" + 255 + ";" + 255 + "m");
        if (cursor.y > 0)
        {
            for (int i = 0; i < scale + 1; i++)
            {
                Console.SetCursorPosition(cursor.x + scale + 1 + 2 * i, cursor.y - 1);
                Console.Write("_");
            }
        }
        for (int i = 0; i < scale; i++)
        {
            Console.SetCursorPosition(cursor.x + scale - 1 - i, cursor.y + i);
            Console.Write("/");
            Console.SetCursorPosition(cursor.x + hexSize.x - scale - 1 + i, cursor.y + i);
            Console.Write("\\");
        }
        if (cursor.x > 0)
        {
            Console.SetCursorPosition(cursor.x - 1, cursor.y + scale);
            Console.Write("/");
            Console.SetCursorPosition(cursor.x - 1, cursor.y + hexSize.y - scale - 1);
            Console.Write("\\");
        }
        Console.SetCursorPosition(cursor.x + hexSize.x - 1, cursor.y + scale);
        Console.Write("\\");
        Console.SetCursorPosition(cursor.x + hexSize.x - 1, cursor.y + hexSize.y - scale - 1);
        Console.Write("/");
        for (int i = 0; i < scale; i++)
        {
            Console.SetCursorPosition(cursor.x + i, cursor.y + hexSize.y - scale + i);
            Console.Write("\\");
            Console.SetCursorPosition(cursor.x + hexSize.x - 2 - i, cursor.y + hexSize.y - scale + i);
            Console.Write("/");
        }
        for (int i = 0; i < scale + 1; i++)
        {
            Console.SetCursorPosition(cursor.x + scale + 1 + 2 * i, cursor.y + hexSize.y - 1);
            Console.Write("_");
        }
        Console.SetCursorPosition(Console.WindowWidth - 1, Console.WindowHeight - 1);
    }

    public static void NotCursor(Hex hex)
    {
        CountCursorPosition(hex.coordinates.x, hex.coordinates.y);
        if (cursor.y > 0)
        {
            for (int i = 0; i < scale + 1; i++)
            {
                Console.SetCursorPosition(cursor.x + scale + 1 + 2 * i, cursor.y - 1);
                Console.Write(" ");
            }
        }
        for (int i = 0; i < scale; i++)
        {
            Console.SetCursorPosition(cursor.x + scale - 1 - i, cursor.y + i);
            Console.Write(" ");
            Console.SetCursorPosition(cursor.x + hexSize.x - scale - 1 + i, cursor.y + i);
            Console.Write(" ");
        }
        if (cursor.x > 0)
        {
            Console.SetCursorPosition(cursor.x - 1, cursor.y + scale);
            Console.Write(" ");
            Console.SetCursorPosition(cursor.x - 1, cursor.y + hexSize.y - scale - 1);
            Console.Write(" ");
        }
        Console.SetCursorPosition(cursor.x + hexSize.x - 1, cursor.y + scale);
        Console.Write(" ");
        Console.SetCursorPosition(cursor.x + hexSize.x - 1, cursor.y + hexSize.y - scale - 1);
        Console.Write(" ");
        for (int i = 0; i < scale; i++)
        {
            Console.SetCursorPosition(cursor.x + i, cursor.y + hexSize.y - scale + i);
            Console.Write(" ");
            Console.SetCursorPosition(cursor.x + hexSize.x - 2 - i, cursor.y + hexSize.y - scale + i);
            Console.Write(" ");
        }
        for (int i = 0; i < scale + 1; i++)
        {
            Console.SetCursorPosition(cursor.x + scale + 1 + 2 * i, cursor.y + hexSize.y - 1);
            Console.Write(" ");
        }
    }
    
    public static void Grid(Hex hex)
    {
        CountCursorPosition(hex.coordinates.x, hex.coordinates.y);
        Console.Write("\x1b[38;2;" + 135 + ";" + 135 + ";" + 135 + "m");
        if (cursor.y > 0)
        {
            for (int i = 0; i < scale + 1; i++)
            {
                Console.SetCursorPosition(cursor.x + scale + 1 + 2 * i, cursor.y - 1);
                Console.Write("_");
            }
        }
        for (int i = 0; i < scale; i++)
        {
            Console.SetCursorPosition(cursor.x + scale - 1 - i, cursor.y + i);
            Console.Write("/");
            Console.SetCursorPosition(cursor.x + hexSize.x - scale - 1 + i, cursor.y + i);
            Console.Write("\\");
        }
        if (cursor.x > 0)
        {
            Console.SetCursorPosition(cursor.x - 1, cursor.y + scale);
            Console.Write("/");
            Console.SetCursorPosition(cursor.x - 1, cursor.y + hexSize.y - scale - 1);
            Console.Write("\\");
        }
        Console.SetCursorPosition(cursor.x + hexSize.x - 1, cursor.y + scale);
        Console.Write("\\");
        Console.SetCursorPosition(cursor.x + hexSize.x - 1, cursor.y + hexSize.y - scale - 1);
        Console.Write("/");
        for (int i = 0; i < scale; i++)
        {
            Console.SetCursorPosition(cursor.x + i, cursor.y + hexSize.y - scale + i);
            Console.Write("\\");
            Console.SetCursorPosition(cursor.x + hexSize.x - 2 - i, cursor.y + hexSize.y - scale + i);
            Console.Write("/");
        }
        for (int i = 0; i < scale + 1; i++)
        {
            Console.SetCursorPosition(cursor.x + scale + 1 + 2 * i, cursor.y + hexSize.y - 1);
            Console.Write("_");
        }
        Console.SetCursorPosition(Console.WindowWidth - 1, Console.WindowHeight - 1);
    }

    public static void NotGrid(Hex hex)
    {
        CountCursorPosition(hex.coordinates.x, hex.coordinates.y);
        if (cursor.y > 0)
        {
            for (int i = 0; i < scale + 1; i++)
            {
                Console.SetCursorPosition(cursor.x + scale + 1 + 2 * i, cursor.y - 1);
                Console.Write(" ");
            }
        }
        for (int i = 0; i < scale; i++)
        {
            Console.SetCursorPosition(cursor.x + scale - 1 - i, cursor.y + i);
            Console.Write(" ");
            Console.SetCursorPosition(cursor.x + hexSize.x - scale - 1 + i, cursor.y + i);
            Console.Write(" ");
        }
        if (cursor.x > 0)
        {
            Console.SetCursorPosition(cursor.x - 1, cursor.y + scale);
            Console.Write(" ");
            Console.SetCursorPosition(cursor.x - 1, cursor.y + hexSize.y - scale - 1);
            Console.Write(" ");
        }
        Console.SetCursorPosition(cursor.x + hexSize.x - 1, cursor.y + scale);
        Console.Write(" ");
        Console.SetCursorPosition(cursor.x + hexSize.x - 1, cursor.y + hexSize.y - scale - 1);
        Console.Write(" ");
        for (int i = 0; i < scale; i++)
        {
            Console.SetCursorPosition(cursor.x + i, cursor.y + hexSize.y - scale + i);
            Console.Write(" ");
            Console.SetCursorPosition(cursor.x + hexSize.x - 2 - i, cursor.y + hexSize.y - scale + i);
            Console.Write(" ");
        }
        for (int i = 0; i < scale + 1; i++)
        {
            Console.SetCursorPosition(cursor.x + scale + 1 + 2 * i, cursor.y + hexSize.y - 1);
            Console.Write(" ");
        }
    }

    public static void SetScreenSize()
    {
        for (int i = Console.WindowWidth / (hexSize.x - scale); i >= 0; i--)
        {
            if ((hexSize.x - scale) * i + scale < Console.WindowWidth || (hexSize.x - scale) * i + scale == Console.WindowWidth && scale != 0)
            {
                screenSize.x = i;
                break;
            }
        }
        for (int i = Console.WindowHeight / hexSize.y; i >= 0; i--)
        {
            if (hexSize.y * i + hexSize.y / 2 <= Console.WindowHeight)
            {
                screenSize.y = i;
                break;
            }
        }
    }

    public static void CountCursorPosition(int x, int y)
    {
        cursor.x = (x - offset.x) * (hexSize.x - scale);
        if (x % 2 == 0)
        {
            cursor.y = (y - offset.y) * hexSize.y;
        }
        else
        {
            cursor.y = (y - offset.y) * hexSize.y + hexSize.y / 2;
        }
    }
    
    public static void PrintChar(Hex hex)
    {
        switch (hex.terrain)
        {
            case Terrain.Water:
                Console.Write("\x1b[38;2;" + 0 + ";" + 0 + ";" + 200 + "m~ ");
                break;
            case Terrain.Plain:
                Console.Write("\x1b[38;2;" + 0 + ";" + 200 + ";" + 0 + "m~ ");
                break;
            case Terrain.Desert:
                Console.Write("\x1b[38;2;" + 200 + ";" + 200 + ";" + 0 + "m~ ");
                break;
            case Terrain.Forest:
                Console.Write("\x1b[38;2;" + 0 + ";" + 175 + ";" + 0 + "m@ ");
                break;
            case Terrain.PlainHills:
                Console.Write("\x1b[38;2;" + 0 + ";" + 200 + ";" + 0 + "mA ");
                break;
            case Terrain.DesertHills:
                Console.Write("\x1b[38;2;" + 200 + ";" + 200 + ";" + 0 + "mA ");
                break;
            case Terrain.Mountains:
                Console.Write("\x1b[38;2;" + 175 + ";" + 175 + ";" + 175 + "mA ");
                break;
        }
    }
    
    public static void HalfPrintChar(Hex hex)
    {
        switch (hex.terrain)
        {
            case Terrain.Water:
                Console.Write("\x1b[38;2;" + 0 + ";" + 0 + ";" + 200 + "m~");
                break;
            case Terrain.Plain:
                Console.Write("\x1b[38;2;" + 0 + ";" + 200 + ";" + 0 + "m~");
                break;
            case Terrain.Desert:
                Console.Write("\x1b[38;2;" + 200 + ";" + 200 + ";" + 0 + "m~");
                break;
            case Terrain.Forest:
                Console.Write("\x1b[38;2;" + 0 + ";" + 175 + ";" + 0 + "m@");
                break;
            case Terrain.PlainHills:
                Console.Write("\x1b[38;2;" + 0 + ";" + 200 + ";" + 0 + "mA");
                break;
            case Terrain.DesertHills:
                Console.Write("\x1b[38;2;" + 200 + ";" + 200 + ";" + 0 + "mA");
                break;
            case Terrain.Mountains:
                Console.Write("\x1b[38;2;" + 175 + ";" + 175 + ";" + 175 + "mA");
                break;
        }
    }
    
    public static void SetBackgroundColor(Hex hex)
    {
        switch (hex.civ)
        {
            case Civ.Red:
                Console.Write("\x1b[48;2;" + 100 + ";" + 0 + ";" + 0 + "m");
                break;
            case Civ.Blue:
                Console.Write("\x1b[48;2;" + 0 + ";" + 0 + ";" + 100 + "m");
                break;
            default:
                Console.Write("\x1b[48;2;" + 0 + ";" + 0 + ";" + 0 + "m");
                break;
        }
    }
}