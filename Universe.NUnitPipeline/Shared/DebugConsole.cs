using System;

namespace Universe.NUnitPipeline.Shared
{
    internal class DebugConsole
    {
        public static void WriteLine(string message)
        {
            // return;
            ConsoleColor? fc = null;
            try
            {
                fc = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(message);
                Console.ForegroundColor = fc.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine(message);
            }
            finally
            {
                if (fc.HasValue) Console.ForegroundColor = fc.Value;
            }
        }
    }
}