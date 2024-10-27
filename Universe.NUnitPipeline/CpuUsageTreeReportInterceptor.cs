using System;
using System.IO;
using System.Text;
using NUnit.Framework.Interfaces;
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

				// TODO: 
				TestResult testResult = test.GetPropertyOrAdd<TestResult>("Test Result", null);
				// TestResult testResult = null;

				var reportRow = new DetailsReport()
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
				// Build and Store Plain Table
				string plainReport = null;
				try
				{
					var rawList = DetailsReportStorage.Instance.GetRawRows();
					ConsoleTable ct = new ConsoleTable("Test", "Status",
						"-Duration", "-CPU (%)", "-CPU (ms)", "-User", "-Kernel",
						"Message"
					);

					foreach (var detail in rawList)
					{
						bool hasCpuUsage = detail.UserTime.HasValue || detail.KernelTime.HasValue;
						double totalCpuUsage = detail.UserTime.GetValueOrDefault() + detail.KernelTime.GetValueOrDefault();
						double? percents = detail.Duration > 0 ? totalCpuUsage / detail.Duration : (double?)null;
						ct.AddRow(
							detail.Key.ToString(),
							detail.OutcomeStatus,
							Math.Round(1000 * detail.Duration, 1),
							percents * 100,
							totalCpuUsage,
							1000 * detail.UserTime,
							1000 * detail.KernelTime,
							detail.ErrorMessage
						);
					}

					plainReport = ct.ToString();
				}
				catch (Exception ex)
				{
					plainReport = $"Report Build Failed{Environment.NewLine}{ex}";
				}

				var filePath = NUnitPipelineChain.InternalReportFile + ".Plain.Summary.txt";
				TryAndForget.Execute(() => Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(filePath))));
				using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
				using (StreamWriter wr = new StreamWriter(fs, new UTF8Encoding(false)))
				{
					wr.Write(plainReport);
				}

			}

		}
	}
}
