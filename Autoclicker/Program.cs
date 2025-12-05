using System.Runtime.InteropServices;

class Program
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll")]
    private static extern bool PeekMessage(out MSG msg, IntPtr hWnd, uint filterMin, uint filterMax, uint flags);

    [DllImport("user32.dll")]
    private static extern void mouse_event(uint flags, uint dx, uint dy, uint data, UIntPtr extraInfo);

    const uint LEFTDOWN = 0x02;
    const uint LEFTUP = 0x04;
    const uint RIGHTDOWN = 0x08;
    const uint RIGHTUP = 0x10;

    static bool running = false;
    static bool isRightButton = false;
    static int delay = 100;

    static void Main()
    {
        RegisterHotKey(IntPtr.Zero, 0, 0, (uint)ConsoleKey.F5);
        RegisterHotKey(IntPtr.Zero, 1, 0, (uint)ConsoleKey.F6);
        RegisterHotKey(IntPtr.Zero, 2, 0, (uint)ConsoleKey.F7);
        RegisterHotKey(IntPtr.Zero, 3, 0, (uint)ConsoleKey.F8);
        RegisterHotKey(IntPtr.Zero, 4, 0, (uint)ConsoleKey.F9);

        PrintMenu();

        while (true)
        {
            if (PeekMessage(out MSG msg, IntPtr.Zero, 0, 0, 1))
            {
                if (msg.message == 0x0312)
                {
                    switch ((int)msg.wParam)
                    {
                        case 0:
                            isRightButton = !isRightButton;
                            Console.Clear();
                            PrintMenu();
                            break;
                        case 1:
                            running = true;
                            break;
                        case 2:
                            running = false;
                            break;
                        case 3:
                            running = false;
                            Console.Clear();
                            ChangeInterval();
                            Console.Clear();
                            PrintMenu();
                            break;
                        case 4:
                            UnregisterAllHotkeys();
                            Environment.Exit(0);
                            break;
                    }
                }
            }

            if (running)
            {
                Click();
                Thread.Sleep(delay);
            }
        }
    }

    static void Click()
    {
        if (isRightButton)
        {
            mouse_event(RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
            mouse_event(RIGHTUP, 0, 0, 0, UIntPtr.Zero);
        }
        else
        {
            mouse_event(LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
            mouse_event(LEFTUP, 0, 0, 0, UIntPtr.Zero);
        }
    }

    static void ChangeInterval()
    {
        Console.Write("Interval between clicks (ms): ");
        if (!int.TryParse(Console.ReadLine(), out int newDelay))
        {
            Console.Clear();
            Console.WriteLine("Invalid number.");
            ChangeInterval();
        }
        else if (newDelay <= 0)
        {
            Console.Clear();
            Console.WriteLine("Delay too short. Must be at least 1 ms.");
            ChangeInterval();
        }
        else
        {
            delay = newDelay;
        }
    }

    static void PrintMenu()
    {
        WriteAsciiCentered("                      _            _                _   _        _                                 \r\n  ___   ___   ___    /_\\    _  _  | |_   ___   __  | | (_)  __  | |__  ___   _ _   ___   ___   ___ \r\n |___| |___| |___|  / _ \\  | || | |  _| / _ \\ / _| | | | | / _| | / / / -_) | '_| |___| |___| |___|\r\n                   /_/ \\_\\  \\_,_|  \\__| \\___/ \\__| |_| |_| \\__| |_\\_\\ \\___| |_|                    \r\n                                                                                                   ");

        Console.WriteLine(
                "                      ___            ___   _         _     _     _            ___   _                                   \r\n  ___   ___   ___    | _ )  _  _    | _ \\ | |  ___  | |_  | |__ (_)  _ _     |_ _| | |  _  _   __ _     ___   ___   ___ \r\n |___| |___| |___|   | _ \\ | || |   |  _/ | | / _ \\ |  _| | / / | | | ' \\     | |  | | | || | / _` |   |___| |___| |___|\r\n                     |___/  \\_, |   |_|   |_| \\___/  \\__| |_\\_\\ |_| |_||_|   |___| |_|  \\_, | \\__,_|                    \r\n                            |__/                                                        |__/                            \n");

        Console.WriteLine("F5 = toggle right/left mouse button (currently " + (isRightButton ? "right" : "left") + ").");
        Console.WriteLine("F6 = start autoclicker");
        Console.WriteLine("F7 = stop autoclicker");
        Console.WriteLine($"F8 = change delay (currently: {delay})");
        Console.WriteLine("F9 = exit");
    }

    static void UnregisterAllHotkeys()
    {
        for (int i = 0; i <= 4; i++)
            UnregisterHotKey(IntPtr.Zero, i);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public System.Drawing.Point pt;
    }

    static void WriteAsciiCentered(string ascii)
    {
        string[] lines = ascii.Split('\n');
        int consoleWidth = Console.WindowWidth;
        int maxLen = 0;
        foreach (var line in lines)
            maxLen = Math.Max(maxLen, line.Replace("\r", "").Length);
        foreach (var rawLine in lines)
        {
            string line = rawLine.Replace("\r", "");
            int leftPad = (consoleWidth - maxLen) / 2;
            if (leftPad < 0) leftPad = 0;
            Console.Write(new string(' ', leftPad));
            Console.WriteLine(line);
        }
    }
}