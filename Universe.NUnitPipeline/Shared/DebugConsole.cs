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
			lock (InternalLog.Sync) InternalLog.Buffer.AppendLine(message);
			Console.WriteLine(message);
			return;
		}
	}
	internal class DebugConsole
    {
        public static void WriteLine(string message)
        {
	        lock (InternalLog.Sync) InternalLog.InternalBuffer.AppendLine(message);
	        Console.WriteLine(message);
            return;
			ConsoleColor? fc = null;
            try
            {
                fc = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(message);
                Console.ForegroundColor = fc.Value;
            }
            catch (Exception)
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
