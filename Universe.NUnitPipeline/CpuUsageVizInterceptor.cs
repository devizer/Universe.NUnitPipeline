extern alias nunit;
using nunit::NUnit.Framework.Interfaces;
using System;
using nunit::NUnit.Framework;
using Universe.NUnitPipeline.Shared;

namespace Universe.NUnitPipeline
{
	extern alias nunit;

	public class CpuUsageResult
    {
        public TimeSpan Elapsed { get; set; }
        public CpuUsage.CpuUsage? CpuUsage { get; set; }
    }


    public static class CpuUsageVizInterceptor {
        
        public static readonly string Title = "Cpu Usage Visualizer";
        static bool IsFirstEmptyLine = true;

		public static void OnStart(NUnitStage stage, ITest test)
        {
            if (stage.NUnitActionAppliedTo != NUnitActionAppliedTo.Test) return;

            string emptyLine = null;
            if (IsFirstEmptyLine)
	            IsFirstEmptyLine = false;
			else
	            emptyLine = Environment.NewLine;

            PipelineLog.WriteLine($"{emptyLine}→ {stage.FormattedIndex} {stage.FixtureFullName}::{stage.TestName} is starting");
        }
        public static void OnFinish(NUnitStage stage, ITest test)
        {
            if (stage.NUnitActionAppliedTo != NUnitActionAppliedTo.Test) return;
            
            var cpuUsageResult = test.GetPropertyOrAdd<CpuUsageResult>(nameof(CpuUsageResult), null);
            if (cpuUsageResult == null) return;

            // Visualize
            var finalCpuUsage = cpuUsageResult.CpuUsage.GetValueOrDefault();
            var hasCpuUsage = cpuUsageResult.CpuUsage.HasValue;
            var elapsed = cpuUsageResult.Elapsed;

            double user = finalCpuUsage.UserUsage.TotalMicroSeconds / 1000d;
            double kernel = finalCpuUsage.KernelUsage.TotalMicroSeconds / 1000d;
            double perCents = elapsed.TotalSeconds == 0d ? 0 : (user + kernel) / 1000d / elapsed.TotalSeconds;

            string elapsedFormatted = ElapsedFormatter.FormatElapsed(elapsed);

            bool alreadyHasMilliseconds = elapsedFormatted.IndexOf("millisecond", StringComparison.OrdinalIgnoreCase) >= 0;
            string cpuUsageHumanized =
                hasCpuUsage
                    ? $" (cpu: {perCents * 100:f0}%, {user + kernel:n3} = {user:n3} [user] + {kernel:n3} [kernel]{(!alreadyHasMilliseconds ? " milliseconds" : "")})"
                    : null;

            string outcomeStatus = TestContext.CurrentContext.Result.Outcome.Status.ToString().ToUpper();
            string resultMessage = TestContext.CurrentContext.Result.Message;

			bool isOk = "PASSED".Equals(outcomeStatus, StringComparison.OrdinalIgnoreCase);
            var outcomeStatusColored = HighlightedOutcomeStatus(outcomeStatus);

            var formattedMessage = isOk || string.IsNullOrEmpty(resultMessage) ? "" : $"{Environment.NewLine}  Details: {resultMessage}";
			PipelineLog.WriteLine($"← {stage.FormattedIndex} {stage.FixtureFullName}::{AsYellow(stage.TestName)} >{outcomeStatusColored}< in {elapsedFormatted}{cpuUsageHumanized}{formattedMessage}");
        }

        static string AsYellow(string arg)
        {
            char esc = (char)27;
            return AnsiSupportInfo.IsAnsiSupported
                ? $"{esc}[33m{arg}{esc}[0m"
                : arg;
        }

        static string HighlightedOutcomeStatus(string outcomeStatus)
        {
            if (AnsiSupportInfo.IsAnsiSupported)
            {
                char esc = (char)27;
                bool isOk = "PASSED".Equals(outcomeStatus, StringComparison.OrdinalIgnoreCase);
                string color = isOk ? "[92m" : "[31m";
                outcomeStatus = $"{esc}{color}{outcomeStatus}{esc}[0m";
            }

            return outcomeStatus;
        }
    }
}
