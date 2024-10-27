using System;
using System.Text;

namespace Universe.NUnitPipeline.Shared
{
	internal class InternalLog
	{
		internal static readonly StringBuilder Buffer = new StringBuilder();
		internal static readonly StringBuilder InternalBuffer = new StringBuilder();
		internal static readonly object Sync = new object();

	}
	internal class OutputConsole
	{
		public static void WriteLine(string message)
		{
			lock (InternalLog.Sync)
			{
				InternalLog.InternalBuffer.AppendLine(message);
				InternalLog.Buffer.AppendLine(message);
			}
			Console.WriteLine(message);
		}
	}
	internal class DebugConsole
    {
        public static void WriteLine(string message)
        {
	        lock (InternalLog.Sync) InternalLog.InternalBuffer.AppendLine(message);
        }

        private static void ColorfulWriteLine(string message)
        {
	        ConsoleColor? fc = null;
	        try
	        {
		        fc = Console.ForegroundColor;
		        Console.ForegroundColor = ConsoleColor.DarkYellow;
		        Console.WriteLine((string)message);
		        Console.ForegroundColor = fc.Value;
	        }
	        catch (Exception)
	        {
		        Console.WriteLine((string)message);
	        }
	        finally
	        {
		        if (fc.HasValue) Console.ForegroundColor = fc.Value;
	        }
        }
    }
}
