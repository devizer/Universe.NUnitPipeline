extern alias nunit;
using nunit::NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Universe.GenericTreeTable;
using Universe.NUnitPipeline.ConsoleTreeTable;
using Universe.NUnitPipeline.Shared;


namespace Universe.NUnitPipeline
{
	public static class CpuUsageTreeReportInterceptor
	{
		public static readonly string Title = "Cpu Usage Tree Report";

		public static void OnFinish(NUnitStage stage, ITest test)
		{
			if (stage.NUnitActionAppliedTo == NUnitActionAppliedTo.Test)
			{
				// Enlist Details
				CpuUsageResult cpuUsageResult = test.GetPropertyOrAdd<CpuUsageResult>(nameof(CpuUsageResult), null);
				if (cpuUsageResult == null) return;

				TestResult testResult = test.GetPropertyOrAdd<TestResult>("Test Result", null);

				var reportRow = new DetailsReportRow()
				{
					Key = new TreeKey(stage.StructuredFullName),
					Duration = cpuUsageResult.Elapsed.TotalSeconds,
					UserTime = cpuUsageResult.CpuUsage?.UserUsage.TotalSeconds,
					KernelTime = cpuUsageResult.CpuUsage?.KernelUsage.TotalSeconds,
					ErrorMessage = testResult?.ResultMessage,
					OutcomeStatus = testResult?.ResultOutcome ?? "Incomplete"
				};
				DetailsReportStorage.Instance.EnlistRow(reportRow);
			}

			if (stage.NUnitActionAppliedTo == NUnitActionAppliedTo.Assembly)
			{
				var internalReportFile = NUnitPipelineConfiguration.GetService<NUnitReportConfiguration>().InternalReportFile;
				if (string.IsNullOrEmpty(internalReportFile)) return;
					
				// Build and Store Plain Table
				var plainReport = BuildPlainReport();
				var plainReportFileName = internalReportFile + ".Plain.Summary.txt";
				FileEx.WriteAll(plainReportFileName, plainReport);

				var treeReport = BuildTreeReport();
				var treeReportFileName = internalReportFile + ".Tree.Summary.txt";
				FileEx.WriteAll(treeReportFileName, treeReport);
			}

		}

		private static string BuildTreeReport()
		{
			string treeReport = null;
			try
			{
				List<DetailsReportRow> reportCopyRaw = DetailsReportStorage.Instance.GetRawRows();
				var builder = new TreeTableBuilder<string, DetailsReportRow>(TestsTreeConfiguration.Instance);
				reportCopyRaw = reportCopyRaw.OrderBy(x => x.Key.ToString()).ToList();
				var consoleTable = builder.Build(reportCopyRaw.Select(x => new KeyValuePair<IEnumerable<string>, DetailsReportRow>(x.Key.Path, x)));
				return consoleTable.ToString();
			}
			catch (Exception ex)
			{
				treeReport = $"Report Build Failed{Environment.NewLine}{ex}";
			}

			return treeReport;
		}

		private static string BuildPlainReport()
		{
			string plainReport = null;
			try
			{
				List<DetailsReportRow> reportCopyRaw = DetailsReportStorage.Instance.GetRawRows();
				var builder = new TreeTableBuilder<string, DetailsReportRow>(TestsTreeConfiguration.Instance);
				reportCopyRaw = reportCopyRaw.OrderBy(x => x.Key.ToString()).ToList();
				var consoleTable = builder.BuildPlain(reportCopyRaw.Select(x => new KeyValuePair<IEnumerable<string>, DetailsReportRow>(x.Key.Path, x)));
				return consoleTable.ToString();
			}
			catch (Exception ex)
			{
				plainReport = $"Report Build Failed{Environment.NewLine}{ex}";
			}

			return plainReport;
		}
	}
}
