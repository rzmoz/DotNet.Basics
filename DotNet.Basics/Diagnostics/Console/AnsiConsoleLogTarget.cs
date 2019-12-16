using System;
using System.Runtime.InteropServices;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Diagnostics.Console
{
    public class AnsiConsoleLogTarget : ConsoleLogTarget
    {
        private readonly ConsoleTheme _consoleTheme;

        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        public AnsiConsoleLogTarget(ConsoleTheme consoleTheme = null)
        {
            _consoleTheme = consoleTheme ?? ConsoleTheme.Default;
        }

        public static bool IsSupported
        {
            get
            {
                var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
                if (!GetConsoleMode(iStdOut, out uint outConsoleMode))
                    return false;

                outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
                return SetConsoleMode(iStdOut, outConsoleMode);
            }
        }

        protected override string FormatLogOutput(LogLevel level, string message, Exception e = null)
        {
            var format = _consoleTheme.Get(level);
            return base.FormatLogOutput(level, message, e).AnsiColorize(format);
        }
    }
}
