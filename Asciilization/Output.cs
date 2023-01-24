using System.Text;

namespace Asciilization;

public class Output
{
    public static Coordinates hexSize;
    public static Coordinates screenSize;
    public static Coordinates offset;
    public static int scale;
    public static Coordinates cursor;
    public static string[,] matrix;
    public static int backColor;
    public static int foreColor;
    public static string ch;
    public static StringBuilder sb;
    public static int k;
    public static int bU;
    public static int bD;
    public static int bL;
    public static int bR;
    public static int delta;
    
    public static void Init(int hexSizeX, int hexSizeY, int offsetX, int offsetY, int sc)
    {
        hexSize = new Coordinates(hexSizeX, hexSizeY);
        offset = new Coordinates(offsetX, offsetY);
        scale = sc;
        SetScreenSize();
        matrix = new string[Console.WindowHeight, Console.WindowWidth];
        sb = new StringBuilder();
    }
    
    public static void Init(int hexSizeX, int hexSizeY, int sc)
    {
        hexSize = new Coordinates(hexSizeX, hexSizeY);
        scale = sc;
        SetScreenSize();
    }
    
    public static void Map(Map map)
    {
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
        foreach (string s in matrix)
        {
            sb.Append(s);
        }
        Console.Write(sb);
    }
    
    public static void Hex(Hex hex)
    {
        CountCursorPosition(hex.coord.x, hex.coord.y);
        backColor = SetBackgroundColor(hex);
        foreColor = SetForegroundColor(hex);
        ch = SetChar(hex);
        for (int i = 0; i < hexSize.y / 2; i++)
        {
            for (int j = 0; j < hexSize.x - 2 * (scale - i); j++)
            {
                if (j % 2 == 0)
                {
                    matrix[cursor.y + i, cursor.x + scale - i + j] = "\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch;
                }
                else
                {
                    matrix[cursor.y + i, cursor.x + scale - i + j] = "\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + " ";
                }
            }
            for (int j = 0; j < hexSize.x - 2 * i; j++)
            {
                if (j % 2 == 0)
                {
                    matrix[cursor.y + hexSize.y / 2 + i, cursor.x + i + j] = "\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch;
                }
                else
                {
                    matrix[cursor.y + hexSize.y / 2 + i, cursor.x + i + j] = "\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + " ";
                }
            }
        }
    }
    
    public static void HalfHex(Hex hex)
    {
        CountCursorPosition(hex.coord.x, hex.coord.y);
        backColor = SetBackgroundColor(hex);
        foreColor = SetForegroundColor(hex);
        ch = SetChar(hex);
        bU = 0;
        bD = 0;
        bL = 0;
        bR = 0;
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
            k = Math.Abs(scale - i) - i / (hexSize.y / 2);
            if (Console.WindowWidth - cursor.x < hexSize.x)
            {
                bR = hexSize.x - (Console.WindowWidth - cursor.x + k);
            }
            if (cursor.x < 0)
            {
                bL = -(cursor.x + k);
            }
            if (Console.WindowWidth - scale == cursor.x && k == scale)
            {
                continue;
            }
            for (int j = bL; j < hexSize.x - bR - 2 * k; j++)
            {
                if (j % 2 == 0)
                {
                    matrix[cursor.y + i, cursor.x + k + j] = "\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch;
                }
                else
                {
                    matrix[cursor.y + i, cursor.x + k + j] = "\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + " ";
                }
            }
        }
    }
    
    public static void FantomHex(int x, int y)
    {
        CountCursorPosition(x, y);
        bU = 0;
        bD = 0;
        bL = 0;
        bR = 0;
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
            k = Math.Abs(scale - i) - i / (hexSize.y / 2);
            if (Console.WindowWidth - cursor.x < hexSize.x)
            {
                bR = hexSize.x - (Console.WindowWidth - cursor.x + k);
            }
            if (cursor.x < 0)
            {
                bL = -(cursor.x + k);
            }
            if (Console.WindowWidth - scale == cursor.x && k == scale)
            {
                continue;
            }
            for (int j = bL; j < hexSize.x - bR - 2 * k; j++)
            {
                if (j % 2 == 0)
                {
                    matrix[cursor.y + i, cursor.x + k + j] = "\x1b[48;5;" + 16 + "m\x1b[38;5;" + 20 + "m" + "~";
                }
                else
                {
                    matrix[cursor.y + i, cursor.x + k + j] = "\x1b[48;5;" + 16 + "m\x1b[38;5;" + 20 + "m" + " ";
                }
            }
        }
    }
    
    public static void Rivers(Map map)
    {
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
        CountCursorPosition(hex.coord.x, hex.coord.y);
        if (hex.coord.x % 2 == 0)
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
                        backColor = SetBackgroundColor(hex);
                        matrix[cursor.y, cursor.x + scale + 2 * i] = "\x1b[48;5;" + backColor + "m\x1b[38;5;" + 20 + "m~";
                    }
                    if (cursor.y - 1 >= 0 && cursor.y - 1 < Console.WindowHeight)
                    {
                        backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1, hex.coord.x]);
                        matrix[cursor.y - 1, cursor.x + scale + 2 * i] = "\x1b[48;5;" + backColor + "m\x1b[38;5;" + 20 + "m~";
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
                        backColor = SetBackgroundColor(hex);
                        matrix[cursor.y + i, cursor.x + scale - i] = "\x1b[48;5;" + backColor + "m\x1b[38;5;" + 20 + "m~";
                    }
                    if (cursor.x + scale - i - 2 >= 0 && cursor.x + scale - i - 2 < Console.WindowWidth)
                    {
                        backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1 + delta, hex.coord.x - 1]);
                        matrix[cursor.y + i, cursor.x + scale - i - 2] = "\x1b[48;5;" + backColor + "m\x1b[38;5;" + 20 + "m~";
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
                        backColor = SetBackgroundColor(hex);
                        matrix[cursor.y + hexSize.y / 2 + i, cursor.x + i] = "\x1b[48;5;" + backColor + "m\x1b[38;5;" + 20 + "m~";
                    }

                    if (cursor.x + i - 2 >= 0 && cursor.x + i - 2 < Console.WindowWidth)
                    {
                        backColor = SetBackgroundColor(map.hexes[hex.coord.y + delta, hex.coord.x - 1]);
                        matrix[cursor.y + hexSize.y / 2 + i, cursor.x + i - 2] = "\x1b[48;5;" + backColor + "m\x1b[38;5;" + 20 + "m~";
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
                        backColor = SetBackgroundColor(hex);
                        matrix[cursor.y + hexSize.y - 1, cursor.x + scale + 2 * i] = "\x1b[48;5;" + backColor + "m\x1b[38;5;" + 20 + "m~";
                    }
                    if (cursor.y + hexSize.y >= 0 && cursor.y + hexSize.y < Console.WindowHeight)
                    {
                        backColor = SetBackgroundColor(map.hexes[hex.coord.y + 1, hex.coord.x]);
                        matrix[cursor.y + hexSize.y, cursor.x + scale + 2 * i] = "\x1b[48;5;" + backColor + "m\x1b[38;5;" + 20 + "m~";
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
                        backColor = SetBackgroundColor(hex);
                        matrix[cursor.y + hexSize.y - 1 - i, cursor.x + hexSize.x - scale - 2 + i] = "\x1b[48;5;" + backColor + "m\x1b[38;5;" + 20 + "m~";
                    }
                    if (cursor.x + hexSize.x - scale + i >= 0 && cursor.x + hexSize.x - scale + i < Console.WindowWidth)
                    {
                        backColor = SetBackgroundColor(map.hexes[hex.coord.y + delta, hex.coord.x + 1]);
                        matrix[cursor.y + hexSize.y - 1 - i, cursor.x + hexSize.x - scale + i] = "\x1b[48;5;" + backColor + "m\x1b[38;5;" + 20 + "m~";
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
                        backColor = SetBackgroundColor(hex);
                        matrix[cursor.y + hexSize.y / 2 - 1 - i, cursor.x + hexSize.x - 2 - i] = "\x1b[48;5;" + backColor + "m\x1b[38;5;" + 20 + "m~";
                    }

                    if (cursor.x + hexSize.x - i >= 0 && cursor.x + hexSize.x - i < Console.WindowWidth)
                    {
                        backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1 + delta, hex.coord.x + 1]);
                        matrix[cursor.y + hexSize.y / 2 - 1 - i, cursor.x + hexSize.x - i] = "\x1b[48;5;" + backColor + "m\x1b[38;5;" + 20 + "m~";
                    }
                }
            }
        }
    }
    
    public static void Cursor(Hex hex)
    {
        CountCursorPosition(hex.coord.x, hex.coord.y);
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
        CountCursorPosition(hex.coord.x, hex.coord.y);
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
        CountCursorPosition(hex.coord.x, hex.coord.y);
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
        CountCursorPosition(hex.coord.x, hex.coord.y);
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
    
    public static string SetChar(Hex hex)
    {
        switch (hex.terrain)
        {
            case Terrain.Forest:
                return "@";
            case Terrain.PlainHills: case Terrain.DesertHills: case Terrain.Mountains:
                return "A";
            default:
                return "~";
        }
    }
    
    public static int SetForegroundColor(Hex hex)
    {
        switch (hex.terrain)
        {
            case Terrain.Plain: case Terrain.PlainHills:
                return 40;
            case Terrain.Desert: case Terrain.DesertHills:
                return 184;
            case Terrain.Forest:
                return 34;
            case Terrain.Mountains:
                return 145;
            default:
                return 20;
        }
    }
    
    public static int SetBackgroundColor(Hex hex)
    {
        switch (hex.civ)
        {
            case Civ.Red:
                return 88;
            case Civ.Orange:
                return 130;
            case Civ.Cian:
                return 30;
            case Civ.Magenta:
                return 91;
            default:
                return 16;
        }
    }
}