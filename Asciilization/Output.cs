using System.Text;

namespace Asciilization;

public class Output
{
    public static Coord hexSize;
    public static Coord screenSize;
    public static Coord offset;
    public static int scale;
    public static Coord cursor;
    public static int delta;
    public static string backColor;
    public static string foreColor;
    public static string ch;
    public static string[] symbols;
    public static string mapBorder;
    public static string unitColor;
    public static StringBuilder hexesLayer;
    public static StringBuilder riversLayer;
    public static StringBuilder uiLayer;
    public static StringBuilder gridLayer;
    
    public static void Init(int hexSizeX, int hexSizeY, int offsetX, int offsetY, int sc)
    {
        hexSize = new Coord(hexSizeX, hexSizeY);
        offset = new Coord(offsetX, offsetY);
        scale = sc;
        SetScreenSize();
        symbols = new string[3];
        mapBorder = "\x1b[48;5;016m\x1b[38;5;020m";
        hexesLayer = new StringBuilder();
        riversLayer = new StringBuilder();
        uiLayer = new StringBuilder();
        gridLayer = new StringBuilder();
        InitSymbols();
    }
    
    public static void Init(int hexSizeX, int hexSizeY, int sc)
    {
        hexSize = new Coord(hexSizeX, hexSizeY);
        scale = sc;
        SetScreenSize();
        InitSymbols();
    }
    
    public static void Map(Map map)
    {
        Hexes(map);
        Rivers(map);
        UI(map);
        Console.Write(hexesLayer);
        Console.SetCursorPosition(0, 0);
        Console.Write(riversLayer);
        Console.SetCursorPosition(0, 0);
        Console.Write(uiLayer);
    }

    public static void Hexes(Map map)
    {
        int indentL;
        int lim1;
        int lim2;
        int indentR;
        if (offset.x % 2 == 0)
        {
            for (int i = offset.y; i < screenSize.y + offset.y; i++)
            {
                indentL = scale;
                for (int k = 0; k < hexSize.y / 2; k++)
                {
                    if (offset.x > 0)
                    {
                        if ((offset.x - 1) % 2 == 0)
                        {
                            backColor = SetBackgroundColor(map.hexes[i, offset.x - 1]);
                            foreColor = SetForegroundColor(map.hexes[i, offset.x - 1]);
                            ch = SetChar(map.hexes[i, offset.x - 1], hexSize.x - indentL);
                            hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                        }
                        else if (i > 0)
                        {
                            backColor = SetBackgroundColor(map.hexes[i - 1, offset.x - 1]);
                            foreColor = SetForegroundColor(map.hexes[i - 1, offset.x - 1]);
                            ch = SetChar(map.hexes[i - 1, offset.x - 1], hexSize.x - indentL);
                            hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                        }
                        else
                        {
                            ch = SetChar(map.hexes[0, 0], hexSize.x - indentL);
                            hexesLayer.Append(mapBorder + ch);
                        }
                    }
                    else
                    {
                        ch = SetChar(map.hexes[0, 0], hexSize.x - indentL);
                        hexesLayer.Append(mapBorder + ch);
                    }
                    lim1 = 2 * indentL;
                    lim2 = 2 * (scale - indentL);
                    for (int j = offset.x; j < screenSize.x + offset.x; j++)
                    {
                        if (j % 2 == 0)
                        {
                            backColor = SetBackgroundColor(map.hexes[i, j]);
                            foreColor = SetForegroundColor(map.hexes[i, j]);
                            ch = SetChar(map.hexes[i, j], lim1);
                            hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                        }
                        else if (i > 0)
                        {
                            backColor = SetBackgroundColor(map.hexes[i - 1, j]);
                            foreColor = SetForegroundColor(map.hexes[i - 1, j]);
                            ch = SetChar(map.hexes[i - 1, j], lim2);
                            hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                        }
                        else
                        {
                            ch = SetChar(map.hexes[0, 0], lim2);
                            hexesLayer.Append(mapBorder + ch);
                        }
                    }
                    for (int j = screenSize.x + offset.x; j < screenSize.x + offset.x + 2; j++)
                    {
                        if ((j - offset.x) % 2 == 0)
                        {
                            indentR = Console.WindowWidth - ((hexSize.x - scale) * (j - offset.x) + scale) + (scale - indentL);
                        }
                        else
                        {
                            indentR = Console.WindowWidth - ((hexSize.x - scale) * (j - offset.x) + scale) + indentL;
                        }
                        if (j % 2 == 0 && indentR > hexSize.x - lim1)
                        {
                            indentR = hexSize.x - lim1;
                        }
                        else if (j % 2 != 0 && indentR > hexSize.x - lim2)
                        {
                            indentR = hexSize.x - lim2;
                        }
                        if (indentR > 0)
                        {
                            if (j < map.hexes.GetLength(1))
                            {
                                if (j % 2 == 0)
                                {
                                    backColor = SetBackgroundColor(map.hexes[i, j]);
                                    foreColor = SetForegroundColor(map.hexes[i, j]);
                                    ch = SetChar(map.hexes[i, j], 0, indentR);
                                    hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                                }
                                else if (i > 0)
                                {
                                    backColor = SetBackgroundColor(map.hexes[i - 1, j]);
                                    foreColor = SetForegroundColor(map.hexes[i - 1, j]);
                                    ch = SetChar(map.hexes[i - 1, j], 0, indentR);
                                    hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                                }
                                else
                                {
                                    ch = SetChar(map.hexes[0, 0], 0, indentR);
                                    hexesLayer.Append(mapBorder + ch);
                                }
                            }
                            else
                            {
                                ch = SetChar(map.hexes[0, 0], 0, indentR);
                                hexesLayer.Append(mapBorder + ch);
                            }
                        }
                    }
                    indentL--;
                }
                indentL = 0;
                for (int k = hexSize.y / 2; k < hexSize.y; k++)
                {
                    if (offset.x > 0)
                    {
                        backColor = SetBackgroundColor(map.hexes[i, offset.x - 1]);
                        foreColor = SetForegroundColor(map.hexes[i, offset.x - 1]);
                        ch = SetChar(map.hexes[i, offset.x - 1], hexSize.x - indentL);
                        hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                    }
                    else
                    {
                        ch = SetChar(map.hexes[0, 0], hexSize.x - indentL);
                        hexesLayer.Append(mapBorder + ch);
                    }
                    lim1 = 2 * indentL;
                    lim2 = 2 * (scale - indentL);
                    for (int j = offset.x; j < screenSize.x + offset.x; j++)
                    {
                        backColor = SetBackgroundColor(map.hexes[i, j]);
                        foreColor = SetForegroundColor(map.hexes[i, j]);
                        if (j % 2 == 0)
                        {
                            ch = SetChar(map.hexes[i, j], lim1);
                        }
                        else
                        {
                            ch = SetChar(map.hexes[i, j], lim2);
                        }
                        hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                    }
                    for (int j = screenSize.x + offset.x; j < screenSize.x + offset.x + 2; j++)
                    {
                        if ((j - offset.x) % 2 == 0)
                        {
                            indentR = Console.WindowWidth - ((hexSize.x - scale) * (j - offset.x) + scale) + (scale - indentL);
                        }
                        else
                        {
                            indentR = Console.WindowWidth - ((hexSize.x - scale) * (j - offset.x) + scale) + indentL;
                        }
                        if (j % 2 == 0 && indentR > hexSize.x - lim1)
                        {
                            indentR = hexSize.x - lim1;
                        }
                        else if (j % 2 != 0 && indentR > hexSize.x - lim2)
                        {
                            indentR = hexSize.x - lim2;
                        }
                        if (indentR > 0)
                        {
                            if (j < map.hexes.GetLength(1))
                            {
                                backColor = SetBackgroundColor(map.hexes[i, j]);
                                foreColor = SetForegroundColor(map.hexes[i, j]);
                                ch = SetChar(map.hexes[i, j], 0, indentR);
                                hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                            }
                            else
                            {
                                ch = SetChar(map.hexes[0, 0], 0, indentR);
                                hexesLayer.Append(mapBorder + ch);
                            }
                        }
                    }
                    indentL++;
                }
            }
            for (int i = screenSize.y + offset.y; i < screenSize.y + offset.y + 2; i++)
            {
                indentL = scale;
                for (int k = 0; k < hexSize.y / 2; k++)
                {
                    if ((i - offset.y) * hexSize.y + k == Console.WindowHeight)
                    {
                        return;
                    }
                    if (offset.x > 0)
                    {
                        if (i < map.hexes.GetLength(0))
                        {
                            if ((offset.x - 1) % 2 == 0)
                            {
                                backColor = SetBackgroundColor(map.hexes[i, offset.x - 1]);
                                foreColor = SetForegroundColor(map.hexes[i, offset.x - 1]);
                                ch = SetChar(map.hexes[i, offset.x - 1], hexSize.x - indentL);
                                hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                            }
                            else
                            {
                                backColor = SetBackgroundColor(map.hexes[i - 1, offset.x - 1]);
                                foreColor = SetForegroundColor(map.hexes[i - 1, offset.x - 1]);
                                ch = SetChar(map.hexes[i - 1, offset.x - 1], hexSize.x - indentL);
                                hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                            }
                        }
                        else
                        {
                            if (i - 1 < map.hexes.GetLength(0))
                            {
                                backColor = SetBackgroundColor(map.hexes[i - 1, offset.x - 1]);
                                foreColor = SetForegroundColor(map.hexes[i - 1, offset.x - 1]);
                                ch = SetChar(map.hexes[i - 1, offset.x - 1], hexSize.x - indentL);
                                hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                            }
                            else
                            {
                                ch = SetChar(map.hexes[0, 0], hexSize.x - indentL);
                                hexesLayer.Append(mapBorder + ch);
                            }
                        }
                    }
                    else
                    {
                        ch = SetChar(map.hexes[0, 0], hexSize.x - indentL);
                        hexesLayer.Append(mapBorder + ch);
                    }
                    lim1 = 2 * indentL;
                    lim2 = 2 * (scale - indentL);
                    for (int j = offset.x; j < screenSize.x + offset.x; j++)
                    {
                        if (i < map.hexes.GetLength(0))
                        {
                            if (j % 2 == 0)
                            {
                                backColor = SetBackgroundColor(map.hexes[i, j]);
                                foreColor = SetForegroundColor(map.hexes[i, j]);
                                ch = SetChar(map.hexes[i, j], lim1);
                                hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                            }
                            else
                            {
                                backColor = SetBackgroundColor(map.hexes[i - 1, j]);
                                foreColor = SetForegroundColor(map.hexes[i - 1, j]);
                                ch = SetChar(map.hexes[i - 1, j], lim2);
                                hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                            }
                        }
                        else
                        {
                            if (j % 2 == 0)
                            {
                                ch = SetChar(map.hexes[0, 0], lim1);
                                hexesLayer.Append(mapBorder + ch);
                            }
                            else if (i - 1 < map.hexes.GetLength(0))
                            {
                                backColor = SetBackgroundColor(map.hexes[i - 1, j]);
                                foreColor = SetForegroundColor(map.hexes[i - 1, j]);
                                ch = SetChar(map.hexes[i - 1, j], lim2);
                                hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                            }
                            else
                            {
                                ch = SetChar(map.hexes[0, 0], lim2);
                                hexesLayer.Append(mapBorder + ch);
                            }
                        }
                    }
                    for (int j = screenSize.x + offset.x; j < screenSize.x + offset.x + 2; j++)
                    {
                        if ((j - offset.x) % 2 == 0)
                        {
                            indentR = Console.WindowWidth - ((hexSize.x - scale) * (j - offset.x) + scale) + (scale - indentL);
                        }
                        else
                        {
                            indentR = Console.WindowWidth - ((hexSize.x - scale) * (j - offset.x) + scale) + indentL;
                        }
                        if (j % 2 == 0 && indentR > hexSize.x - lim1)
                        {
                            indentR = hexSize.x - lim1;
                        }
                        else if (j % 2 != 0 && indentR > hexSize.x - lim2)
                        {
                            indentR = hexSize.x - lim2;
                        }
                        if (indentR > 0)
                        {
                            if (i < map.hexes.GetLength(0))
                            {
                                if (j < map.hexes.GetLength(1))
                                {
                                    if (j % 2 == 0)
                                    {
                                        backColor = SetBackgroundColor(map.hexes[i, j]);
                                        foreColor = SetForegroundColor(map.hexes[i, j]);
                                        ch = SetChar(map.hexes[i, j], 0, indentR);
                                        hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                                    }
                                    else
                                    {
                                        backColor = SetBackgroundColor(map.hexes[i - 1, j]);
                                        foreColor = SetForegroundColor(map.hexes[i - 1, j]);
                                        ch = SetChar(map.hexes[i - 1, j], 0, indentR);
                                        hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                                    }
                                }
                                else
                                {
                                    ch = SetChar(map.hexes[0, 0], 0, indentR);
                                    hexesLayer.Append(mapBorder + ch);
                                }
                            }
                            else
                            {
                                if (j % 2 != 0 && i - 1 < map.hexes.GetLength(0) && j < map.hexes.GetLength(1))
                                {
                                    backColor = SetBackgroundColor(map.hexes[i - 1, j]);
                                    foreColor = SetForegroundColor(map.hexes[i - 1, j]);
                                    ch = SetChar(map.hexes[i - 1, j], 0, indentR);
                                    hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                                }
                                else
                                {
                                    ch = SetChar(map.hexes[0, 0], 0, indentR);
                                    hexesLayer.Append(mapBorder + ch);
                                }
                            }
                        }
                    }
                    indentL--;
                }
                indentL = 0;
                for (int k = hexSize.y / 2; k < hexSize.y; k++)
                {
                    if ((i - offset.y) * hexSize.y + k == Console.WindowHeight)
                    {
                        return;
                    }
                    if (offset.x > 0 && i < map.hexes.GetLength(0))
                    {
                        backColor = SetBackgroundColor(map.hexes[i, offset.x - 1]);
                        foreColor = SetForegroundColor(map.hexes[i, offset.x - 1]);
                        ch = SetChar(map.hexes[i, offset.x - 1], hexSize.x - indentL);
                        hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                    }
                    else
                    {
                        ch = SetChar(map.hexes[0, 0], hexSize.x - indentL);
                        hexesLayer.Append(mapBorder + ch);
                    }
                    lim1 = 2 * indentL;
                    lim2 = 2 * (scale - indentL);
                    for (int j = offset.x; j < screenSize.x + offset.x; j++)
                    {
                        if (i < map.hexes.GetLength(0))
                        {
                            backColor = SetBackgroundColor(map.hexes[i, j]);
                            foreColor = SetForegroundColor(map.hexes[i, j]);
                            if (j % 2 == 0)
                            {
                                ch = SetChar(map.hexes[i, j], lim1);
                            }
                            else
                            {
                                ch = SetChar(map.hexes[i, j], lim2);
                            }
                            hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                        }
                        else
                        {
                            if (j % 2 == 0)
                            {
                                ch = SetChar(map.hexes[0, 0], lim1);
                            }
                            else
                            {
                                ch = SetChar(map.hexes[0, 0], lim2);
                            }
                            hexesLayer.Append(mapBorder + ch);
                        }
                    }
                    for (int j = screenSize.x + offset.x; j < screenSize.x + offset.x + 2; j++)
                    {
                        if ((j - offset.x) % 2 == 0)
                        {
                            indentR = Console.WindowWidth - ((hexSize.x - scale) * (j - offset.x) + scale) + (scale - indentL);
                        }
                        else
                        {
                            indentR = Console.WindowWidth - ((hexSize.x - scale) * (j - offset.x) + scale) + indentL;
                        }
                        if (j % 2 == 0 && indentR > hexSize.x - lim1)
                        {
                            indentR = hexSize.x - lim1;
                        }
                        else if (j % 2 != 0 && indentR > hexSize.x - lim2)
                        {
                            indentR = hexSize.x - lim2;
                        }
                        if (indentR > 0)
                        {
                            if (i < map.hexes.GetLength(0) && j < map.hexes.GetLength(1))
                            {
                                backColor = SetBackgroundColor(map.hexes[i, j]);
                                foreColor = SetForegroundColor(map.hexes[i, j]);
                                ch = SetChar(map.hexes[i, j], 0, indentR);
                                hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                            }
                            else
                            {
                                ch = SetChar(map.hexes[0, 0], 0, indentR);
                                hexesLayer.Append(mapBorder + ch);
                            }
                        }
                    }
                    indentL++;
                }
            }
        }
        else
        {
            for (int i = offset.y; i < screenSize.y + offset.y; i++)
            {
                indentL = 0;
                for (int k = 0; k < hexSize.y / 2; k++)
                {
                    if (offset.x > 0)
                    {
                        if ((offset.x - 1) % 2 == 0)
                        {
                            backColor = SetBackgroundColor(map.hexes[i, offset.x - 1]);
                            foreColor = SetForegroundColor(map.hexes[i, offset.x - 1]);
                            ch = SetChar(map.hexes[i, offset.x - 1], hexSize.x - indentL);
                            hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                        }
                        else if (i > 0)
                        {
                            backColor = SetBackgroundColor(map.hexes[i - 1, offset.x - 1]);
                            foreColor = SetForegroundColor(map.hexes[i - 1, offset.x - 1]);
                            ch = SetChar(map.hexes[i - 1, offset.x - 1], hexSize.x - indentL);
                            hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                        }
                        else
                        {
                            ch = SetChar(map.hexes[0, 0], hexSize.x - indentL);
                            hexesLayer.Append(mapBorder + ch);
                        }
                    }
                    else
                    {
                        ch = SetChar(map.hexes[0, 0], hexSize.x - indentL);
                        hexesLayer.Append(mapBorder + ch);
                    }
                    lim1 = 2 * indentL;
                    lim2 = 2 * (scale - indentL);
                    for (int j = offset.x; j < screenSize.x + offset.x; j++)
                    {
                        if (j % 2 == 0)
                        {
                            backColor = SetBackgroundColor(map.hexes[i, j]);
                            foreColor = SetForegroundColor(map.hexes[i, j]);
                            ch = SetChar(map.hexes[i, j], lim2);
                            hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                        }
                        else if (i > 0)
                        {
                            backColor = SetBackgroundColor(map.hexes[i - 1, j]);
                            foreColor = SetForegroundColor(map.hexes[i - 1, j]);
                            ch = SetChar(map.hexes[i - 1, j], lim1);
                            hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                        }
                        else
                        {
                            ch = SetChar(map.hexes[0, 0], lim1);
                            hexesLayer.Append(mapBorder + ch);
                        }
                    }
                    for (int j = screenSize.x + offset.x; j < screenSize.x + offset.x + 2; j++)
                    {
                        if ((j - offset.x) % 2 == 0)
                        {
                            indentR = Console.WindowWidth - ((hexSize.x - scale) * (j - offset.x) + scale) + (scale - indentL);
                        }
                        else
                        {
                            indentR = Console.WindowWidth - ((hexSize.x - scale) * (j - offset.x) + scale) + indentL;
                        }
                        if (j % 2 == 0 && indentR > hexSize.x - lim2)
                        {
                            indentR = hexSize.x - lim2;
                        }
                        else if (j % 2 != 0 && indentR > hexSize.x - lim1)
                        {
                            indentR = hexSize.x - lim1;
                        }
                        if (indentR > 0)
                        {
                            if (j < map.hexes.GetLength(1))
                            {
                                if (j % 2 == 0)
                                {
                                    backColor = SetBackgroundColor(map.hexes[i, j]);
                                    foreColor = SetForegroundColor(map.hexes[i, j]);
                                    ch = SetChar(map.hexes[i, j], 0, indentR);
                                    hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                                }
                                else if (i > 0)
                                {
                                    backColor = SetBackgroundColor(map.hexes[i - 1, j]);
                                    foreColor = SetForegroundColor(map.hexes[i - 1, j]);
                                    ch = SetChar(map.hexes[i - 1, j], 0, indentR);
                                    hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                                }
                                else
                                {
                                    ch = SetChar(map.hexes[0, 0], 0, indentR);
                                    hexesLayer.Append(mapBorder + ch);
                                }
                            }
                            else
                            {
                                ch = SetChar(map.hexes[0, 0], 0, indentR);
                                hexesLayer.Append(mapBorder + ch);
                            }
                        }
                    }
                    indentL++;
                }
                indentL = scale;
                for (int k = hexSize.y / 2; k < hexSize.y; k++)
                {
                    if (offset.x > 0)
                    {
                        backColor = SetBackgroundColor(map.hexes[i, offset.x - 1]);
                        foreColor = SetForegroundColor(map.hexes[i, offset.x - 1]);
                        ch = SetChar(map.hexes[i, offset.x - 1], hexSize.x - indentL);
                        hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                    }
                    else
                    {
                        ch = SetChar(map.hexes[0, 0], hexSize.x - indentL);
                        hexesLayer.Append(mapBorder + ch);
                    }
                    lim1 = 2 * indentL;
                    lim2 = 2 * (scale - indentL);
                    for (int j = offset.x; j < screenSize.x + offset.x; j++)
                    {
                        backColor = SetBackgroundColor(map.hexes[i, j]);
                        foreColor = SetForegroundColor(map.hexes[i, j]);
                        if (j % 2 == 0)
                        {
                            ch = SetChar(map.hexes[i, j], lim2);
                        }
                        else
                        {
                            ch = SetChar(map.hexes[i, j], lim1);
                        }
                        hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                    }
                    for (int j = screenSize.x + offset.x; j < screenSize.x + offset.x + 2; j++)
                    {
                        if ((j - offset.x) % 2 == 0)
                        {
                            indentR = Console.WindowWidth - ((hexSize.x - scale) * (j - offset.x) + scale) + (scale - indentL);
                        }
                        else
                        {
                            indentR = Console.WindowWidth - ((hexSize.x - scale) * (j - offset.x) + scale) + indentL;
                        }
                        if (j % 2 == 0 && indentR > hexSize.x - lim2)
                        {
                            indentR = hexSize.x - lim2;
                        }
                        else if (j % 2 != 0 && indentR > hexSize.x - lim1)
                        {
                            indentR = hexSize.x - lim1;
                        }
                        if (indentR > 0)
                        {
                            if (j < map.hexes.GetLength(1))
                            {
                                backColor = SetBackgroundColor(map.hexes[i, j]);
                                foreColor = SetForegroundColor(map.hexes[i, j]);
                                ch = SetChar(map.hexes[i, j], 0, indentR);
                                hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                            }
                            else
                            {
                                ch = SetChar(map.hexes[0, 0], 0, indentR);
                                hexesLayer.Append(mapBorder + ch);
                            }
                        }
                    }
                    indentL--;
                }
            }
            for (int i = screenSize.y + offset.y; i < screenSize.y + offset.y + 2; i++)
            {
                indentL = 0;
                for (int k = 0; k < hexSize.y / 2; k++)
                {
                    if ((i - offset.y) * hexSize.y + k == Console.WindowHeight)
                    {
                        return;
                    }
                    if (offset.x > 0)
                    {
                        if (i < map.hexes.GetLength(0))
                        {
                            if ((offset.x - 1) % 2 == 0)
                            {
                                backColor = SetBackgroundColor(map.hexes[i, offset.x - 1]);
                                foreColor = SetForegroundColor(map.hexes[i, offset.x - 1]);
                                ch = SetChar(map.hexes[i, offset.x - 1], hexSize.x - indentL);
                                hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                            }
                            else
                            {
                                backColor = SetBackgroundColor(map.hexes[i - 1, offset.x - 1]);
                                foreColor = SetForegroundColor(map.hexes[i - 1, offset.x - 1]);
                                ch = SetChar(map.hexes[i - 1, offset.x - 1], hexSize.x - indentL);
                                hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                            }
                        }
                        else
                        {
                            if (i - 1 < map.hexes.GetLength(0))
                            {
                                backColor = SetBackgroundColor(map.hexes[i - 1, offset.x - 1]);
                                foreColor = SetForegroundColor(map.hexes[i - 1, offset.x - 1]);
                                ch = SetChar(map.hexes[i - 1, offset.x - 1], hexSize.x - indentL);
                                hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                            }
                            else
                            {
                                ch = SetChar(map.hexes[0, 0], hexSize.x - indentL);
                                hexesLayer.Append(mapBorder + ch);
                            }
                        }
                    }
                    else
                    {
                        ch = SetChar(map.hexes[0, 0], hexSize.x - indentL);
                        hexesLayer.Append(mapBorder + ch);
                    }
                    lim1 = 2 * indentL;
                    lim2 = 2 * (scale - indentL);
                    for (int j = offset.x; j < screenSize.x + offset.x; j++)
                    {
                        if (i < map.hexes.GetLength(0))
                        {
                            if (j % 2 == 0)
                            {
                                backColor = SetBackgroundColor(map.hexes[i, j]);
                                foreColor = SetForegroundColor(map.hexes[i, j]);
                                ch = SetChar(map.hexes[i, j], lim2);
                                hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                            }
                            else
                            {
                                backColor = SetBackgroundColor(map.hexes[i - 1, j]);
                                foreColor = SetForegroundColor(map.hexes[i - 1, j]);
                                ch = SetChar(map.hexes[i - 1, j], lim1);
                                hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                            }
                        }
                        else
                        {
                            if (j % 2 == 0)
                            {
                                ch = SetChar(map.hexes[0, 0], lim2);
                                hexesLayer.Append(mapBorder + ch);
                            }
                            else if (i - 1 < map.hexes.GetLength(0))
                            {
                                backColor = SetBackgroundColor(map.hexes[i - 1, j]);
                                foreColor = SetForegroundColor(map.hexes[i - 1, j]);
                                ch = SetChar(map.hexes[i - 1, j], lim1);
                                hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                            }
                            else
                            {
                                ch = SetChar(map.hexes[0, 0], lim1);
                                hexesLayer.Append(mapBorder + ch);
                            }
                        }
                    }
                    for (int j = screenSize.x + offset.x; j < screenSize.x + offset.x + 2; j++)
                    {
                        if ((j - offset.x) % 2 == 0)
                        {
                            indentR = Console.WindowWidth - ((hexSize.x - scale) * (j - offset.x) + scale) + (scale - indentL);
                        }
                        else
                        {
                            indentR = Console.WindowWidth - ((hexSize.x - scale) * (j - offset.x) + scale) + indentL;
                        }
                        if (j % 2 == 0 && indentR > hexSize.x - lim2)
                        {
                            indentR = hexSize.x - lim2;
                        }
                        else if (j % 2 != 0 && indentR > hexSize.x - lim1)
                        {
                            indentR = hexSize.x - lim1;
                        }
                        if (indentR > 0)
                        {
                            if (i < map.hexes.GetLength(0))
                            {
                                if (j < map.hexes.GetLength(1))
                                {
                                    if (j % 2 == 0)
                                    {
                                        backColor = SetBackgroundColor(map.hexes[i, j]);
                                        foreColor = SetForegroundColor(map.hexes[i, j]);
                                        ch = SetChar(map.hexes[i, j], 0, indentR);
                                        hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                                    }
                                    else
                                    {
                                        backColor = SetBackgroundColor(map.hexes[i - 1, j]);
                                        foreColor = SetForegroundColor(map.hexes[i - 1, j]);
                                        ch = SetChar(map.hexes[i - 1, j], 0, indentR);
                                        hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                                    }
                                }
                                else
                                {
                                    ch = SetChar(map.hexes[0, 0], 0, indentR);
                                    hexesLayer.Append(mapBorder + ch);
                                }
                            }
                            else
                            {
                                if (j % 2 != 0 && i - 1 < map.hexes.GetLength(0) && j < map.hexes.GetLength(1))
                                {
                                    backColor = SetBackgroundColor(map.hexes[i - 1, j]);
                                    foreColor = SetForegroundColor(map.hexes[i - 1, j]);
                                    ch = SetChar(map.hexes[i - 1, j], 0, indentR);
                                    hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                                }
                                else
                                {
                                    ch = SetChar(map.hexes[0, 0], 0, indentR);
                                    hexesLayer.Append(mapBorder + ch);
                                }
                            }
                        }
                    }
                    indentL++;
                }
                indentL = scale;
                for (int k = hexSize.y / 2; k < hexSize.y; k++)
                {
                    if ((i - offset.y) * hexSize.y + k == Console.WindowHeight)
                    {
                        return;
                    }
                    if (offset.x > 0 && i < map.hexes.GetLength(0))
                    {
                        backColor = SetBackgroundColor(map.hexes[i, offset.x - 1]);
                        foreColor = SetForegroundColor(map.hexes[i, offset.x - 1]);
                        ch = SetChar(map.hexes[i, offset.x - 1], hexSize.x - indentL);
                        hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                    }
                    else
                    {
                        ch = SetChar(map.hexes[0, 0], hexSize.x - indentL);
                        hexesLayer.Append(mapBorder + ch);
                    }
                    lim1 = 2 * indentL;
                    lim2 = 2 * (scale - indentL);
                    for (int j = offset.x; j < screenSize.x + offset.x; j++)
                    {
                        if (i < map.hexes.GetLength(0))
                        {
                            backColor = SetBackgroundColor(map.hexes[i, j]);
                            foreColor = SetForegroundColor(map.hexes[i, j]);
                            if (j % 2 == 0)
                            {
                                ch = SetChar(map.hexes[i, j], lim2);
                            }
                            else
                            {
                                ch = SetChar(map.hexes[i, j], lim1);
                            }
                            hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                        }
                        else
                        {
                            if (j % 2 == 0)
                            {
                                ch = SetChar(map.hexes[0, 0], lim2);
                            }
                            else
                            {
                                ch = SetChar(map.hexes[0, 0], lim1);
                            }
                            hexesLayer.Append(mapBorder + ch);
                        }
                    }
                    for (int j = screenSize.x + offset.x; j < screenSize.x + offset.x + 2; j++)
                    {
                        if ((j - offset.x) % 2 == 0)
                        {
                            indentR = Console.WindowWidth - ((hexSize.x - scale) * (j - offset.x) + scale) + (scale - indentL);
                        }
                        else
                        {
                            indentR = Console.WindowWidth - ((hexSize.x - scale) * (j - offset.x) + scale) + indentL;
                        }
                        if (j % 2 == 0 && indentR > hexSize.x - lim2)
                        {
                            indentR = hexSize.x - lim2;
                        }
                        else if (j % 2 != 0 && indentR > hexSize.x - lim1)
                        {
                            indentR = hexSize.x - lim1;
                        }
                        if (indentR > 0)
                        {
                            if (i < map.hexes.GetLength(0) && j < map.hexes.GetLength(1))
                            {
                                backColor = SetBackgroundColor(map.hexes[i, j]);
                                foreColor = SetForegroundColor(map.hexes[i, j]);
                                ch = SetChar(map.hexes[i, j], 0, indentR);
                                hexesLayer.Append("\x1b[48;5;" + backColor + "m\x1b[38;5;" + foreColor + "m" + ch);
                            }
                            else
                            {
                                ch = SetChar(map.hexes[0, 0], 0, indentR);
                                hexesLayer.Append(mapBorder + ch);
                            }
                        }
                    }
                    indentL--;
                }
            }
        }
    }
    
    public static void Rivers(Map map)
    {
        for (int i = offset.y - 1; i < screenSize.y + offset.y + 2; i++)
        {
            for (int j = offset.x - 1; j < screenSize.x + offset.x + 2; j++)
            {
                if (i != -1 && i < map.hexes.GetLength(0) && j != -1 && j < map.hexes.GetLength(1))
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
                        riversLayer.Append("\x1b[" + (cursor.y + 1) + ";" + (cursor.x + scale + 2 * i + 1) + "H\x1b[48;5;" + backColor + "m\x1b[38;5;20m~");
                    }
                    if (cursor.y - 1 >= 0 && cursor.y - 1 < Console.WindowHeight)
                    {
                        backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1, hex.coord.x]);
                        riversLayer.Append("\x1b[" + cursor.y + ";" + (cursor.x + scale + 2 * i + 1) + "H\x1b[48;5;" + backColor + "m\x1b[38;5;20m~");
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
                        riversLayer.Append("\x1b[" + (cursor.y + i + 1) + ";" + (cursor.x + scale - i + 1) + "H\x1b[48;5;" + backColor + "m\x1b[38;5;20m~");
                    }
                    if (cursor.x + scale - i - 2 >= 0 && cursor.x + scale - i - 2 < Console.WindowWidth)
                    {
                        backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1 + delta, hex.coord.x - 1]);
                        riversLayer.Append("\x1b[" + (cursor.y + i + 1) + ";" + (cursor.x + scale - i - 1) + "H\x1b[48;5;" + backColor + "m\x1b[38;5;20m~");
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
                        riversLayer.Append("\x1b[" + (cursor.y + hexSize.y / 2 + i + 1) + ";" + (cursor.x + i + 1) + "H\x1b[48;5;" + backColor + "m\x1b[38;5;20m~");
                    }
                    if (cursor.x + i - 2 >= 0 && cursor.x + i - 2 < Console.WindowWidth)
                    {
                        backColor = SetBackgroundColor(map.hexes[hex.coord.y + delta, hex.coord.x - 1]);
                        riversLayer.Append("\x1b[" + (cursor.y + hexSize.y / 2 + i + 1) + ";" + (cursor.x + i - 1) + "H\x1b[48;5;" + backColor + "m\x1b[38;5;20m~");
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
                        riversLayer.Append("\x1b[" + (cursor.y + hexSize.y) + ";" + (cursor.x + scale + 2 * i + 1) + "H\x1b[48;5;" + backColor + "m\x1b[38;5;20m~");
                    }
                    if (cursor.y + hexSize.y >= 0 && cursor.y + hexSize.y < Console.WindowHeight)
                    {
                        backColor = SetBackgroundColor(map.hexes[hex.coord.y + 1, hex.coord.x]);
                        riversLayer.Append("\x1b[" + (cursor.y + hexSize.y + 1) + ";" + (cursor.x + scale + 2 * i + 1) + "H\x1b[48;5;" + backColor + "m\x1b[38;5;20m~");
                    }
                }
            }
        }
        else if (hex.riverDir == 4)
        {
            for (int i = 0; i < hexSize.y / 2; i++)
            {
                if (cursor.y + hexSize.y - i - 1 >= 0 && cursor.y + hexSize.y - i - 1 < Console.WindowHeight)
                {
                    if (cursor.x + hexSize.x - scale + i - 2 >= 0 && cursor.x + hexSize.x - scale + i - 2 < Console.WindowWidth)
                    {
                        backColor = SetBackgroundColor(hex);
                        riversLayer.Append("\x1b[" + (cursor.y + hexSize.y - i) + ";" + (cursor.x + hexSize.x - scale + i - 1) + "H\x1b[48;5;" + backColor + "m\x1b[38;5;20m~");
                    }
                    if (cursor.x + hexSize.x - scale + i >= 0 && cursor.x + hexSize.x - scale + i < Console.WindowWidth)
                    {
                        backColor = SetBackgroundColor(map.hexes[hex.coord.y + delta, hex.coord.x + 1]);
                        riversLayer.Append("\x1b[" + (cursor.y + hexSize.y - i) + ";" + (cursor.x + hexSize.x - scale + i + 1) + "H\x1b[48;5;" + backColor + "m\x1b[38;5;20m~");
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < hexSize.y / 2; i++)
            {
                if (cursor.y + hexSize.y / 2 - i - 1 >= 0 && cursor.y + hexSize.y / 2 - i - 1 < Console.WindowHeight)
                {
                    if (cursor.x + hexSize.x - i - 2 >= 0 && cursor.x + hexSize.x - i - 2 < Console.WindowWidth)
                    {
                        backColor = SetBackgroundColor(hex);
                        riversLayer.Append("\x1b[" + (cursor.y + hexSize.y / 2 - i) + ";" + (cursor.x + hexSize.x - i - 1) + "H\x1b[48;5;" + backColor + "m\x1b[38;5;20m~");
                    }
                    if (cursor.x + hexSize.x - i >= 0 && cursor.x + hexSize.x - i < Console.WindowWidth)
                    {
                        backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1 + delta, hex.coord.x + 1]);
                        riversLayer.Append("\x1b[" + (cursor.y + hexSize.y / 2 - i) + ";" + (cursor.x + hexSize.x - i + 1) + "H\x1b[48;5;" + backColor + "m\x1b[38;5;20m~");
                    }
                }
            }
        }
    }

    public static void UI(Map map)
    {
        uiLayer.Append("\x1b[38;5;231m");
        for (int k = 0; k < Game.civilizations.Length; k++)
        {
            for (int i = offset.y; i < screenSize.y + offset.y; i++)
            {
                for (int j = offset.x; j < screenSize.x + offset.x; j++)
                {
                    if (Game.civilizations[k].units.Count > 0 && Game.civilizations[k].units[0].currentHex == map.hexes[i, j] && scale > 2)
                    {
                        Settler(map.hexes[i, j], (Settler)Game.civilizations[k].units[0]);
                    }
                }
            }
        }
    }

    public static void Settler(Hex hex, Settler settler)
    {
        backColor = SetBackgroundColor(hex);
        unitColor = SetUnitColor(settler);
        CountCursorPosition(hex.coord.x, hex.coord.y);
        uiLayer.Append("\x1b[" + (cursor.y + 2) + ";" + (cursor.x + scale + 3) + "H_\x1b[1C_\x1b[1C_\x1b[1C\x1b[1B\x1b[7D|\x1b[48;5;" + unitColor + "m     \x1b[48;5;" + backColor + "m/\x1b[1B\x1b[7D|\x1b[48;5;" + unitColor + "m_ _ _\x1b[48;5;" + backColor + "m\\" + "\x1b[1B\x1b[7D|\x1b[1B\x1b[1D|");
    }
    
    public static void Cursor(Hex hex, Map map, string color)
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
        Console.Write("\x1b[38;5;" + color + "m");
        if (cursor.y > 0)
        {
            if (hex.coord.y == 0)
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1, hex.coord.x]);
            }
            for (int i = 0; i < scale + 1; i++)
            {
                Console.SetCursorPosition(cursor.x + scale + 2 * i + 1, cursor.y - 1);
                Console.Write("\x1b[48;5;" + backColor + "m_");
            }
        }
        for (int i = 0; i < scale; i++)
        {
            if (hex.coord.y - 1 + delta == -1 || hex.coord.x - 1 == -1)
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1 + delta, hex.coord.x - 1]);
            }
            Console.SetCursorPosition(cursor.x + scale - i - 1, cursor.y + i);
            Console.Write("\x1b[48;5;" + backColor + "m/");
            backColor = SetBackgroundColor(hex);
            Console.SetCursorPosition(cursor.x + hexSize.x - scale + i - 1, cursor.y + i);
            Console.Write("\x1b[48;5;" + backColor + "m\\");
        }
        if (cursor.x > 0)
        {
            if (hex.coord.y - 1 + delta == -1)
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1 + delta, hex.coord.x - 1]);
            }
            Console.SetCursorPosition(cursor.x - 1, cursor.y + scale);
            Console.Write("\x1b[48;5;" + backColor + "m/");
            if (hex.coord.y + delta == map.hexes.GetLength(0))
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y + delta, hex.coord.x - 1]);
            }
            Console.SetCursorPosition(cursor.x - 1, cursor.y + scale + 1);
            Console.Write("\x1b[48;5;" + backColor + "m\\");
        }
        backColor = SetBackgroundColor(hex);
        Console.SetCursorPosition(cursor.x + hexSize.x - 1, cursor.y + scale);
        Console.Write("\x1b[48;5;" + backColor + "m\\");
        Console.SetCursorPosition(cursor.x + hexSize.x - 1, cursor.y + scale + 1);
        Console.Write("\x1b[48;5;" + backColor + "m/");
        for (int i = 0; i < scale; i++)
        {
            if (hex.coord.y + delta == map.hexes.GetLength(0) || hex.coord.x - 1 == -1)
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y + delta, hex.coord.x - 1]);
            }
            Console.SetCursorPosition(cursor.x + i, cursor.y + hexSize.y - scale + i);
            Console.Write("\x1b[48;5;" + backColor + "m\\");
            backColor = SetBackgroundColor(hex);
            Console.SetCursorPosition(cursor.x + hexSize.x - i - 2, cursor.y + hexSize.y - scale + i);
            Console.Write("\x1b[48;5;" + backColor + "m/");
        }
        backColor = SetBackgroundColor(hex);
        for (int i = 0; i < scale + 1; i++)
        {
            Console.SetCursorPosition(cursor.x + scale + 2 * i + 1, cursor.y + hexSize.y - 1);
            Console.Write("\x1b[48;5;" + backColor + "m_");
        }
        Console.SetCursorPosition(Console.WindowWidth - 1, Console.WindowHeight - 1);
    }
    
    public static void NotCursor(Hex hex, Map map)
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
        if (cursor.y > 0)
        {
            if (hex.coord.y == 0)
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1, hex.coord.x]);
            }
            for (int i = 0; i < scale + 1; i++)
            {
                Console.SetCursorPosition(cursor.x + scale + 2 * i + 1, cursor.y - 1);
                Console.Write("\x1b[48;5;" + backColor + "m ");
            }
        }
        for (int i = 0; i < scale; i++)
        {
            if (hex.coord.y - 1 + delta == -1 || hex.coord.x - 1 == -1)
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1 + delta, hex.coord.x - 1]);
            }
            Console.SetCursorPosition(cursor.x + scale - i - 1, cursor.y + i);
            Console.Write("\x1b[48;5;" + backColor + "m ");
            backColor = SetBackgroundColor(hex);
            Console.SetCursorPosition(cursor.x + hexSize.x - scale + i - 1, cursor.y + i);
            Console.Write("\x1b[48;5;" + backColor + "m ");
        }
        if (cursor.x > 0)
        {
            if (hex.coord.y - 1 + delta == -1)
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1 + delta, hex.coord.x - 1]);
            }
            Console.SetCursorPosition(cursor.x - 1, cursor.y + scale);
            Console.Write("\x1b[48;5;" + backColor + "m ");
            if (hex.coord.y + delta == map.hexes.GetLength(0))
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y + delta, hex.coord.x - 1]);
            }
            Console.SetCursorPosition(cursor.x - 1, cursor.y + scale + 1);
            Console.Write("\x1b[48;5;" + backColor + "m ");
        }
        backColor = SetBackgroundColor(hex);
        Console.SetCursorPosition(cursor.x + hexSize.x - 1, cursor.y + scale);
        Console.Write("\x1b[48;5;" + backColor + "m ");
        Console.SetCursorPosition(cursor.x + hexSize.x - 1, cursor.y + scale + 1);
        Console.Write("\x1b[48;5;" + backColor + "m ");
        for (int i = 0; i < scale; i++)
        {
            if (hex.coord.y + delta == map.hexes.GetLength(0) || hex.coord.x - 1 == -1)
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y + delta, hex.coord.x - 1]);
            }
            Console.SetCursorPosition(cursor.x + i, cursor.y + hexSize.y - scale + i);
            Console.Write("\x1b[48;5;" + backColor + "m ");
            backColor = SetBackgroundColor(hex);
            Console.SetCursorPosition(cursor.x + hexSize.x - i - 2, cursor.y + hexSize.y - scale + i);
            Console.Write("\x1b[48;5;" + backColor + "m ");
        }
        backColor = SetBackgroundColor(hex);
        for (int i = 0; i < scale + 1; i++)
        {
            Console.SetCursorPosition(cursor.x + scale + 2 * i + 1, cursor.y + hexSize.y - 1);
            Console.Write("\x1b[48;5;" + backColor + "m ");
        }
    }
    
    public static void Grid(Hex hex, Map map)
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
        Console.Write("\x1b[38;5;059m");
        if (cursor.y > 0)
        {
            if (hex.coord.y == 0)
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1, hex.coord.x]);
            }
            for (int i = 0; i < scale + 1; i++)
            {
                Console.SetCursorPosition(cursor.x + scale + 2 * i + 1, cursor.y - 1);
                Console.Write("\x1b[48;5;" + backColor + "m_");
            }
        }
        for (int i = 0; i < scale; i++)
        {
            if (hex.coord.y - 1 + delta == -1 || hex.coord.x - 1 == -1)
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1 + delta, hex.coord.x - 1]);
            }
            Console.SetCursorPosition(cursor.x + scale - i - 1, cursor.y + i);
            Console.Write("\x1b[48;5;" + backColor + "m/");
            backColor = SetBackgroundColor(hex);
            Console.SetCursorPosition(cursor.x + hexSize.x - scale + i - 1, cursor.y + i);
            Console.Write("\x1b[48;5;" + backColor + "m\\");
        }
        if (cursor.x > 0)
        {
            if (hex.coord.y - 1 + delta == -1)
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1 + delta, hex.coord.x - 1]);
            }
            Console.SetCursorPosition(cursor.x - 1, cursor.y + scale);
            Console.Write("\x1b[48;5;" + backColor + "m/");
            if (hex.coord.y + delta == map.hexes.GetLength(0))
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y + delta, hex.coord.x - 1]);
            }
            Console.SetCursorPosition(cursor.x - 1, cursor.y + scale + 1);
            Console.Write("\x1b[48;5;" + backColor + "m\\");
        }
        backColor = SetBackgroundColor(hex);
        Console.SetCursorPosition(cursor.x + hexSize.x - 1, cursor.y + scale);
        Console.Write("\x1b[48;5;" + backColor + "m\\");
        Console.SetCursorPosition(cursor.x + hexSize.x - 1, cursor.y + scale + 1);
        Console.Write("\x1b[48;5;" + backColor + "m/");
        for (int i = 0; i < scale; i++)
        {
            if (hex.coord.y + delta == map.hexes.GetLength(0) || hex.coord.x - 1 == -1)
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y + delta, hex.coord.x - 1]);
            }
            Console.SetCursorPosition(cursor.x + i, cursor.y + hexSize.y - scale + i);
            Console.Write("\x1b[48;5;" + backColor + "m\\");
            backColor = SetBackgroundColor(hex);
            Console.SetCursorPosition(cursor.x + hexSize.x - i - 2, cursor.y + hexSize.y - scale + i);
            Console.Write("\x1b[48;5;" + backColor + "m/");
        }
        backColor = SetBackgroundColor(hex);
        for (int i = 0; i < scale + 1; i++)
        {
            Console.SetCursorPosition(cursor.x + scale + 2 * i + 1, cursor.y + hexSize.y - 1);
            Console.Write("\x1b[48;5;" + backColor + "m_");
        }
    }
    
    public static void SbGrid(Hex hex, Map map)
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
        gridLayer.Append("\x1b[38;5;059m");
        if (cursor.y > 0)
        {
            if (hex.coord.y == 0)
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1, hex.coord.x]);
            }
            for (int i = 0; i < scale + 1; i++)
            {
                gridLayer.Append("\x1b[" + cursor.y + ";" + (cursor.x + scale + 2 * i + 2) + "H\x1b[48;5;" + backColor + "m_");
            }
        }
        for (int i = 0; i < scale; i++)
        {
            if (hex.coord.y - 1 + delta == -1 || hex.coord.x - 1 == -1)
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1 + delta, hex.coord.x - 1]);
            }
            gridLayer.Append("\x1b[" + (cursor.y + i + 1) + ";" + (cursor.x + scale - i) + "H\x1b[48;5;" + backColor + "m/");
            backColor = SetBackgroundColor(hex);
            gridLayer.Append("\x1b[" + (cursor.y + i + 1) + ";" + (cursor.x + hexSize.x - scale + i) + "H\x1b[48;5;" + backColor + "m\\");
        }
        if (cursor.x > 0)
        {
            if (hex.coord.y - 1 + delta == -1)
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1 + delta, hex.coord.x - 1]);
            }
            gridLayer.Append("\x1b[" + (cursor.y + scale + 1) + ";" + cursor.x + "H\x1b[48;5;" + backColor + "m/");
            if (hex.coord.y + delta == map.hexes.GetLength(0))
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y + delta, hex.coord.x - 1]);
            }
            gridLayer.Append("\x1b[" + (cursor.y + hexSize.y - scale) + ";" + cursor.x + "H\x1b[48;5;" + backColor + "m\\");
        }
        backColor = SetBackgroundColor(hex);
        gridLayer.Append("\x1b[" + (cursor.y + scale + 1) + ";" + (cursor.x + hexSize.x) + "H\x1b[48;5;" + backColor + "m\\");
        gridLayer.Append("\x1b[" + (cursor.y + hexSize.y - scale) + ";" + (cursor.x + hexSize.x) + "H\x1b[48;5;" + backColor + "m/");
        for (int i = 0; i < scale; i++)
        {
            if (hex.coord.y + delta == map.hexes.GetLength(0) || hex.coord.x - 1 == -1)
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y + delta, hex.coord.x - 1]);
            }
            gridLayer.Append("\x1b[" + (cursor.y + hexSize.y - scale + i + 1) + ";" + (cursor.x + i + 1) + "H\x1b[48;5;" + backColor + "m\\");
            backColor = SetBackgroundColor(hex);
            gridLayer.Append("\x1b[" + (cursor.y + hexSize.y - scale + i + 1) + ";" + (cursor.x + hexSize.x - i - 1) + "H\x1b[48;5;" + backColor + "m/");
        }
        backColor = SetBackgroundColor(hex);
        for (int i = 0; i < scale + 1; i++)
        {
            gridLayer.Append("\x1b[" + (cursor.y + hexSize.y) + ";" + (cursor.x + scale + 2 * i + 2) + "H\x1b[48;5;" + backColor + "m_");
        }
    }
    
    public static void SbNotGrid(Hex hex, Map map)
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
        gridLayer.Append("\x1b[38;5;102m");
        if (cursor.y > 0)
        {
            if (hex.coord.y == 0)
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1, hex.coord.x]);
            }
            for (int i = 0; i < scale + 1; i++)
            {
                gridLayer.Append("\x1b[" + cursor.y + ";" + (cursor.x + scale + 2 * i + 2) + "H\x1b[48;5;" + backColor + "m ");
            }
        }
        for (int i = 0; i < scale; i++)
        {
            if (hex.coord.y - 1 + delta == -1 || hex.coord.x - 1 == -1)
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1 + delta, hex.coord.x - 1]);
            }
            gridLayer.Append("\x1b[" + (cursor.y + i + 1) + ";" + (cursor.x + scale - i) + "H\x1b[48;5;" + backColor + "m ");
            backColor = SetBackgroundColor(hex);
            gridLayer.Append("\x1b[" + (cursor.y + i + 1) + ";" + (cursor.x + hexSize.x - scale + i) + "H\x1b[48;5;" + backColor + "m ");
        }
        if (cursor.x > 0)
        {
            if (hex.coord.y - 1 + delta == -1)
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y - 1 + delta, hex.coord.x - 1]);
            }
            gridLayer.Append("\x1b[" + (cursor.y + scale + 1) + ";" + cursor.x + "H\x1b[48;5;" + backColor + "m ");
            if (hex.coord.y + delta == map.hexes.GetLength(0))
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y + delta, hex.coord.x - 1]);
            }
            gridLayer.Append("\x1b[" + (cursor.y + hexSize.y - scale) + ";" + cursor.x + "H\x1b[48;5;" + backColor + "m ");
        }
        backColor = SetBackgroundColor(hex);
        gridLayer.Append("\x1b[" + (cursor.y + scale + 1) + ";" + (cursor.x + hexSize.x) + "H\x1b[48;5;" + backColor + "m ");
        gridLayer.Append("\x1b[" + (cursor.y + hexSize.y - scale) + ";" + (cursor.x + hexSize.x) + "H\x1b[48;5;" + backColor + "m ");
        for (int i = 0; i < scale; i++)
        {
            if (hex.coord.y + delta == map.hexes.GetLength(0) || hex.coord.x - 1 == -1)
            {
                backColor = "016";
            }
            else
            {
                backColor = SetBackgroundColor(map.hexes[hex.coord.y + delta, hex.coord.x - 1]);
            }
            gridLayer.Append("\x1b[" + (cursor.y + hexSize.y - scale + i + 1) + ";" + (cursor.x + i + 1) + "H\x1b[48;5;" + backColor + "m ");
            backColor = SetBackgroundColor(hex);
            gridLayer.Append("\x1b[" + (cursor.y + hexSize.y - scale + i + 1) + ";" + (cursor.x + hexSize.x - i - 1) + "H\x1b[48;5;" + backColor + "m ");
        }
        backColor = SetBackgroundColor(hex);
        for (int i = 0; i < scale + 1; i++)
        {
            gridLayer.Append("\x1b[" + (cursor.y + hexSize.y) + ";" + (cursor.x + scale + 2 * i + 2) + "H\x1b[48;5;" + backColor + "m ");
        }
    }
    
    public static void SetScreenSize()
    {
        for (int i = Console.WindowWidth / (hexSize.x - scale); i >= 0; i--)
        {
            if ((hexSize.x - scale) * i + scale <= Console.WindowWidth)
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
    
    public static void InitSymbols()
    {
        symbols[0] = string.Concat(Enumerable.Repeat("~ ", hexSize.x / 2));
        symbols[1] = string.Concat(Enumerable.Repeat("@ ", hexSize.x / 2));
        symbols[2] = string.Concat(Enumerable.Repeat("A ", hexSize.x / 2));
    }
    
    public static string SetChar(Hex hex, int index)
    {
        switch (hex.terrain)
        {
            case Terrain.Forest:
                return symbols[1].Substring(index);
            case Terrain.PlainHills: case Terrain.DesertHills: case Terrain.Mountains:
                return symbols[2].Substring(index);
            default:
                return symbols[0].Substring(index);
        }
    }
    
    public static string SetChar(Hex hex, int index, int length)
    {
        switch (hex.terrain)
        {
            case Terrain.Forest:
                return symbols[1].Substring(index, length);
            case Terrain.PlainHills: case Terrain.DesertHills: case Terrain.Mountains:
                return symbols[2].Substring(index, length);
            default:
                return symbols[0].Substring(index, length);
        }
    }
    
    public static string SetForegroundColor(Hex hex)
    {
        switch (hex.terrain)
        {
            case Terrain.Plain: case Terrain.PlainHills:
                return "040";
            case Terrain.Desert: case Terrain.DesertHills:
                return "184";
            case Terrain.Forest:
                return "034";
            case Terrain.Mountains:
                return "145";
            default:
                return "020";
        }
    }
    
    public static string SetBackgroundColor(Hex hex)
    {
        switch (hex.civColor)
        {
            case CivColors.Red:
                return "160";
            case CivColors.Orange:
                return "208";
            case CivColors.Cian:
                return "038";
            case CivColors.DarkBlue:
                return "057";
            case CivColors.Yellow:
                return "226";
            case CivColors.Green:
                return "118";
            case CivColors.Purple:
                return "128";
            case CivColors.Blue:
                return "026";
            default:
                return "016";
        }
    }
    public static string SetUnitColor(Settler settler)
    {
        switch (settler.owner.name)
        {
            case CivNames.India:
                return "208";
            case CivNames.Greece:
                return "038";
            case CivNames.Persia:
                return "057";
            case CivNames.Egypt:
                return "226";
            case CivNames.China:
                return "118";
            case CivNames.Carthage:
                return "128";
            case CivNames.Assyria:
                return "026";
            default:
                return "160";
        }
    }
}