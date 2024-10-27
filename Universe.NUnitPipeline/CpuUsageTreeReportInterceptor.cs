using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
				// Build and Store Plain Table
				var plainReport = BuildPlainReport();
				var plainReportFileName = NUnitPipelineChain.InternalReportFile + ".Plain.Summary.txt";
				FileEx.WriteAll(plainReportFileName, plainReport);

				var treeReport = BuildTreeReport();
				var treeReportFileName = NUnitPipelineChain.InternalReportFile + ".Tree.Summary.txt";
				FileEx.WriteAll(treeReportFileName, treeReport);
			}

		}

		private static string BuildTreeReport()
		{
			string treeReport = null;
			try
			{
				List<DetailsReportRow> reportCopyRaw = DetailsReportStorage.Instance.GetRawRows();
				reportCopyRaw = reportCopyRaw.OrderBy(x => x.Key.ToString()).ToList();
				var reportCopy = reportCopyRaw.ToDictionary(x => x.Key, x => x);

				List<Node<TreeKey>> rootKeys = TreeKey.AsTree(reportCopyRaw.Select(x => x.Key));
				List<KeyValuePair<TreeKey, string>> orderedKeys = new List<KeyValuePair<TreeKey, string>>();

				void Enum1(List<Node<TreeKey>> nodes)
				{
					foreach (var node in nodes)
					{
						orderedKeys.Add(new KeyValuePair<TreeKey, string>(node.State, node.AscII));
						Enum1(node.Children);
					}
				}
				AscIITreeDiagram<TreeKey>.PopulateAscII(rootKeys);
				Enum1(rootKeys);

				var letsDebug = "ok";


				ConsoleTable ct = new ConsoleTable("Test", "Status",
					"-Duration", "-CPU (%)", "-CPU (ms)", "-User", "-Kernel",
					"Message"
				);

				StringBuilder debugTree = new StringBuilder();
				foreach (var pair in orderedKeys)
				{
					TreeKey path = pair.Key;
					string pathAsString = pair.Value;
					// var total = reportCopyRaw.FirstOrDefault(x => x.Key.Equals(path)).Value ?? zeroMetrics;
					debugTree.AppendLine($"{path,-125} {pathAsString}");
					reportCopy.TryGetValue(path, out DetailsReportRow total);
					var detail = total;
					if (total == null) ct.AddRow(pathAsString);
					else
					{
						bool hasCpuUsage = detail.UserTime.HasValue || detail.KernelTime.HasValue;
						double totalCpuUsage = detail.UserTime.GetValueOrDefault() + detail.KernelTime.GetValueOrDefault();
						double? percents = detail.Duration > 0 ? totalCpuUsage / detail.Duration : (double?)null;
						ct.AddRow(
							pathAsString,
							detail.OutcomeStatus,
							Math.Round(1000 * detail.Duration, 1),
							percents * 100,
							1000 * totalCpuUsage,
							1000 * detail.UserTime,
							1000 * detail.KernelTime,
							detail.ErrorMessage
						);
					}
				}

				return ct.ToString();
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
						1000 * totalCpuUsage,
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

			return plainReport;
		}
	}
}
