using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

class Win32
{
    public enum SendMessageTimeoutFlags
    {
        SMTO_NORMAL = 0x0,
        SMTO_BLOCK = 0x1,
        SMTO_ABORTIFHUNG = 0x2,
        SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
        SMTO_ERRORONEXIT = 0x20,
    }

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(
        string lpClassName,
        string lpWindowName);

    [DllImport("user32.dll")]
    static extern IntPtr FindWindowEx(
        IntPtr parentHandle,
        IntPtr childAfter,
        string lclassName,
        string windowTitle);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessageTimeout(
        IntPtr hWnd,
        uint Msg,
        UIntPtr wParam,
        IntPtr lParam,
        SendMessageTimeoutFlags fuFlags,
        uint uTimeout,
        out UIntPtr lpdwResult);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool EnumWindows(
        EnumWindowsProc lpEnumFunc,
        IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    public static void setOwnership(Form f)
    {
        IntPtr progman = FindWindow("Progman", null);
        if (progman == IntPtr.Zero)
        {
            Console.Write("Could not find handle to Progman window.");
            return;
        }

        UIntPtr result = UIntPtr.Zero;
        IntPtr msgResult = SendMessageTimeout(
            progman,
            0x052C,
            new UIntPtr(0),
            IntPtr.Zero,
            SendMessageTimeoutFlags.SMTO_NORMAL,
            1000,
            out result);
        if (msgResult == IntPtr.Zero)
        {
            Console.Write("Failed to send message to Progman.");
            return;
        }

        IntPtr workerw = IntPtr.Zero;
        EnumWindows(new EnumWindowsProc((tophandle, topparamhandle) =>
        {
            IntPtr p = FindWindowEx(tophandle,
                                        IntPtr.Zero,
                                        "SHELLDLL_DefView",
                                        "");

            if (p != IntPtr.Zero)
            {
                workerw = FindWindowEx(IntPtr.Zero,
                                           tophandle,
                                           "WorkerW",
                                           "");
            }

            return true;
        }), IntPtr.Zero);

        if (workerw == IntPtr.Zero)
        {
            Console.Write("Could not find handle to WorkerW window.");
            return;
        }

        SetParent(f.Handle, workerw);
    }
}