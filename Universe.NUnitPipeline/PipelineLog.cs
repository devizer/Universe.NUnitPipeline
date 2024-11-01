using System;
using System.Text;
/* Unmerged change from project 'Universe.NUnitPipeline (net6.0)'
Before:
namespace Universe.NUnitPipeline.Shared
After:
namespace Universe.NUnitPipeline.NUnitPipeline;
using Universe.NUnitPipeline.Shared
*/


namespace Universe.NUnitPipeline
{
	internal class InternalLog
	{
		internal static readonly StringBuilder Buffer = new StringBuilder();
		internal static readonly StringBuilder InternalBuffer = new StringBuilder();
		internal static readonly object Sync = new object();
	}

	public class PipelineLog
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

		public static void LogTrace(string message)
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
