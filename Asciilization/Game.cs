using System.Runtime.InteropServices;
using WindowsInput;
using WindowsInput.Native;

namespace Asciilization;

public class Game
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

    static void Main(string[] args)
    {
        Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
        Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
        InputSimulator inputSimulator = new InputSimulator();
        inputSimulator.Keyboard.KeyDown(VirtualKeyCode.MENU);
        inputSimulator.Keyboard.KeyDown(VirtualKeyCode.RETURN);
        inputSimulator.Keyboard.KeyUp(VirtualKeyCode.MENU);
        inputSimulator.Keyboard.KeyUp(VirtualKeyCode.RETURN);
        var handle = GetStdHandle(-11);
        int mode;
        GetConsoleMode(handle, out mode);
        SetConsoleMode(handle, mode | 0x4);
        Console.Title = "ASCIILIZATION";
        IntPtr consoleWindow = User32.GetForegroundWindow();
        User32.SetWindowLong(consoleWindow, GWL_EXSTYLE, (int)User32.GetWindowLong(consoleWindow, GWL_EXSTYLE) ^ WS_EX_LAYERED);
        User32.SetLayeredWindowAttributes(consoleWindow, 0, 255, LWA_ALPHA);
        Console.Write("\x1b[48;2;" + 0 + ";" + 0 + ";" + 0 + "m");
        Console.Clear();
        Map map = new Map(100, 50);
        map.Fill();
        Generation.Map(map);
        Generation.Civs(map, 2);
        Printing.Init(16, 8, 0, 0, 3);
        Printing.Map(map);
        Control.Map(map);
    }
}