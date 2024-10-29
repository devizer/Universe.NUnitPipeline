using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universe.GenericTreeTable;

namespace Universe.NUnitPipeline.ConsoleTreeTable
{
	public class TreeKey : TreeKey<string>
	{
		public TreeKey(IEnumerable<string> path) : base(path)
		{
		}

		public override string ToString()
		{
			return string.Join(TestsTreeConfiguration.Instance.Separator, Path);
		}
	}
	internal class TestsTreeConfiguration : ITreeTableConfiguration<string, DetailsReportRow>
	{
		public static readonly TestsTreeConfiguration Instance = new TestsTreeConfiguration();

		public IEqualityComparer<string> EqualityComparer { get; } = StringComparer.Ordinal;
		public string KeyPartToText(string keyPart) => keyPart;

		public string Separator => " \x2192 ";

		public ConsoleTable CreateColumns()
		{
			ConsoleTable ct = new ConsoleTable("Test", "Status",
				"-Duration", "-CPU (%)", "-CPU (ms)", "-User", "-Kernel",
				"Message"
			);

			return ct;
		}

		public void WriteColumns(ConsoleTable table, string renderedKey, DetailsReportRow detail)
		{
			double totalCpuUsage = detail.UserTime.GetValueOrDefault() + detail.KernelTime.GetValueOrDefault();
			double? percents = detail.Duration > 0 ? totalCpuUsage / detail.Duration : (double?)null;
			var outcomeStatus = "Passed".Equals(detail.OutcomeStatus, StringComparison.InvariantCultureIgnoreCase) ? "PASSED" : detail.OutcomeStatus;
			table.AddRow(
				renderedKey,
				outcomeStatus,
				Math.Round(1000 * detail.Duration, 1),
				percents * 100,
				1000 * totalCpuUsage,
				1000 * detail.UserTime,
				1000 * detail.KernelTime,
				detail.ErrorMessage
			);
		}

	}
}
