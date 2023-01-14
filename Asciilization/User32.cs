using System.Runtime.InteropServices;

namespace Asciilization;

public class User32
{
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();
    
    [DllImport("user32.dll")]
    public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
    
    [DllImport("user32.dll")]
    public static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);
    
    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
}