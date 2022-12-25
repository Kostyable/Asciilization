namespace Asciilization;

public class Printing
{
    public static Coordinates hexSize;
    public static Coordinates screenSize;
    public static Coordinates offset;
    public static int scale;

    public static void Init(int hexSizeX, int hexSizeY, int offsetX, int offsetY, int sc)
    {
        hexSize.Init(hexSizeX, hexSizeY);
        offset.Init(offsetX, offsetY);
        scale = sc;
        SetScreenSize();
    }
    
    public static void Init(int hexSizeX, int hexSizeY, int sc)
    {
        hexSize.Init(hexSizeX, hexSizeY);
        scale = sc;
        SetScreenSize();
    } 

    public static void Map(Map map)
    {
        for (int i = offset.y - 1; i < screenSize.y + offset.y + 2; i++)
        {
            for (int j = offset.x - 1; j < screenSize.x + offset.x + 1; j++)
            {
                if (i == -1 || i >= map.hexes.GetLength(0) || j == -1 || j == map.hexes.GetLength(1))
                {
                    FantomHex(j, i);
                }
                else
                {
                    Hex(map.hexes[i, j]); 
                }
            }
        }
    }

    public static void Hex(Hex hex)
    {
        Coordinates cursor = CountCursorPosition(hex.x, hex.y);
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
        SetBackgroundColor(hex);
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
            Console.SetCursorPosition(cursor.x + j + bL, cursor.y + i);
            for (int k = bL; k < hexSize.x - bR - 2 * j; k++)
            {
                if (k % 2 == 0)
                {
                    PrintChar(hex);
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
        Coordinates cursor = CountCursorPosition(x, y);
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
            Console.SetCursorPosition(cursor.x + j + bL, cursor.y + i);
            for (int k = bL; k < hexSize.x - bR - 2 * j; k++)
            {
                if (k % 2 == 0)
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

    public static void Cursor(Hex hex)
    {
        Coordinates cursor = CountCursorPosition(hex.x, hex.y);
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

        if (scale > 0 || hex.x - offset.x != screenSize.x)
        {
            Console.SetCursorPosition(cursor.x + hexSize.x - 1, cursor.y + scale);
            Console.Write("\\");
            Console.SetCursorPosition(cursor.x + hexSize.x - 1, cursor.y + hexSize.y - scale - 1);
            Console.Write("/");
        }
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
        Coordinates cursor = CountCursorPosition(hex.x, hex.y);
        Console.Write("\x1b[38;2;" + 255 + ";" + 255 + ";" + 255 + "m");
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
            if ((hexSize.x - scale) * i < Console.WindowWidth)
            {
                screenSize.x = i;
                break;
            }
        }
        for (int i = Console.WindowHeight / hexSize.y; i >= 0; i--)
        {
            if (hexSize.y * i + hexSize.y / 2 < Console.WindowHeight)
            {
                screenSize.y = i;
                break;
            }
        }
    }

    public static Coordinates CountCursorPosition(int x, int y)
    {
        Coordinates cursor;
        cursor.x = (x - offset.x) * (hexSize.x - scale);
        if (x % 2 == 0)
        {
            cursor.y = (y - offset.y) * hexSize.y;
        }
        else
        {
            cursor.y = (y - offset.y) * hexSize.y + hexSize.y / 2;
        }
        return cursor;
    }

    public static char SetChar(Hex hex)
    {
        if (hex.terrain == Terrain.Water || hex.terrain == Terrain.Plain || hex.terrain == Terrain.Desert)
        {
            return '~';
        }
        if (hex.terrain == Terrain.Forest)
        {
            return '@';
        }
        return 'A';
    }
    
    public static void PrintChar(Hex hex)
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