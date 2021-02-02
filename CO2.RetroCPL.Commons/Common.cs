using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CO2.RetroCPL.Commons
{
    public class Common
    {
        public static void WriteLineConsoleColoured(string text, ConsoleColor color)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = defaultColor;
        }

        public static void WriteConsoleException(Exception ex, bool writeStackTrace = false)
        {
            while (ex != null)
            {
                WriteLineConsoleColoured(ex.Message, ConsoleColor.Red);
                if(writeStackTrace) WriteLineConsoleColoured(ex.StackTrace, ConsoleColor.Red);
                ex = ex.InnerException;
            }
        }
    }
}
