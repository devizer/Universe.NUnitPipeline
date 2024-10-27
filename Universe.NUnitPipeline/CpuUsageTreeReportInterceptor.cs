using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
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
				CpuUsageResult cpuUsageResult = test.GetPropertyOrAdd<CpuUsageResult>(nameof(CpuUsageResult), null);
				if (cpuUsageResult == null) return;

				var reportRow = new DetailsReport()
				{
					Key = new TreeKey(stage.StructuredFullName),
					Duration = cpuUsageResult.Elapsed.TotalSeconds,
					UserTime = cpuUsageResult.CpuUsage?.UserUsage.TotalSeconds,
					KernelTime = cpuUsageResult.CpuUsage?.KernelUsage.TotalSeconds,
				};
				DetailsReportStorage.Instance.EnlistRow(reportRow);
			}

			if (stage.NUnitActionAppliedTo == NUnitActionAppliedTo.Assembly)
			{
				var rawList = DetailsReportStorage.Instance.GetRawRows();
				ConsoleTable ct = new ConsoleTable("Test", "Status",
					"-Duration", "-CPU (%)", "-CPU (\x3bcs)", "-User", "-Kernel"
				);

				foreach (var detail in rawList)
				{
					ct.AddRow(detail.Key.ToString(), "?", Math.Round(detail.Duration, 1), "%%", null, detail.UserTime, detail.KernelTime);
				}

				var filePath = NUnitPipelineChain.InternalReportFile + ".Plain.Summary.txt";
				TryAndForget.Execute(() => Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(filePath))));
				using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
				using (StreamWriter wr = new StreamWriter(fs, new UTF8Encoding(false)))
				{
					wr.Write(ct.ToString());
				}

			}

		}
	}
}
