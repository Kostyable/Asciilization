using System.Runtime.InteropServices;
using WindowsInput;
using WindowsInput.Native;

namespace Asciilization;

public static class Game
{
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
    
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetConsoleMode(IntPtr handle, out int mode);
    
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetStdHandle(int handle);
    
    const int GWL_EXSTYLE = -20;
    const int WS_EX_LAYERED = 0x80000;
    const int LWA_ALPHA = 0x2;
    const int LWA_COLORKEY = 0x1;
    
    public static Map map;
    public static Civilization[] civilizations;
    public static Civilization playerCiv;

    public static void Init()
    {
        map = new Map(100, 50);
        civilizations = new Civilization[Enum.GetNames(typeof(CivNames)).Length];
        for (int i = 0; i < civilizations.Length; i++)
        {
            civilizations[i] = new Civilization((CivNames)i);
        }
        playerCiv = civilizations[Generation.random.Next(Enum.GetNames(typeof(CivNames)).Length)];
    }
    
    public static void Main(string[] args)
    {
        Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
        Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
        InputSimulator inputSimulator = new InputSimulator();
        inputSimulator.Keyboard.KeyDown(VirtualKeyCode.MENU);
        inputSimulator.Keyboard.KeyDown(VirtualKeyCode.RETURN);
        var handle = GetStdHandle(-11);
        int mode;
        GetConsoleMode(handle, out mode);
        SetConsoleMode(handle, mode | 0x4);
        Console.Title = "ASCIILIZATION";
        IntPtr consoleWindow = User32.GetForegroundWindow();
        User32.SetWindowLong(consoleWindow, GWL_EXSTYLE, (int)User32.GetWindowLong(consoleWindow, GWL_EXSTYLE) ^ WS_EX_LAYERED);
        User32.SetLayeredWindowAttributes(consoleWindow, 0, 255, LWA_ALPHA);
        Init();
        Output.Init(16, 8, 0, 0, 3);
        Launch(map, civilizations);
        inputSimulator.Keyboard.KeyUp(VirtualKeyCode.MENU);
        inputSimulator.Keyboard.KeyUp(VirtualKeyCode.RETURN);
        Control.Map(map);
    }
    
    public static void Launch(Map map, Civilization[] civilizations)
    {
        Generation.Map(map);
        Generation.Rivers(map, 7, 5);
        Generation.Settlers(map, civilizations);
        map.Select(playerCiv.units[0].currentHex);
        Control.CursorInCenter(map);
        Output.Map(map);
    }
}