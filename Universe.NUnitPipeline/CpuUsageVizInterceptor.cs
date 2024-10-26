using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Universe.NUnitPipeline.Shared;

namespace Universe.NUnitPipeline
{
    public class CpuUsageResult
    {
        public TimeSpan Elapsed { get; set; }
        public CpuUsage.CpuUsage? CpuUsage { get; set; }
    }

    public static class CpuUsageVizInterceptor {
        
        public static readonly string Title = "Cpu Usage Visualizer";

        public static void OnStart(NUnitStage stage, ITest test)
        {
            if (stage.NUnitActionAppliedTo != NUnitActionAppliedTo.Test) return;

            Console.WriteLine($"→ {stage.FormattedIndex} {stage.FixtureFullName}::{stage.TestName} is starting");
        }
        public static void OnFinish(NUnitStage stage, ITest test)
        {
            if (stage.NUnitActionAppliedTo != NUnitActionAppliedTo.Test) return;
            
            var cpuUsageResult = test.GetPropertyOrAdd<CpuUsageResult>(nameof(CpuUsageResult), null);
            if (cpuUsageResult == null) return;

            var finalCpuUsage = cpuUsageResult.CpuUsage.GetValueOrDefault();
            var hasCpuUsage = cpuUsageResult.CpuUsage.HasValue;
            var elapsed = cpuUsageResult.Elapsed;


            double user = finalCpuUsage.UserUsage.TotalMicroSeconds / 1000d;
            double kernel = finalCpuUsage.KernelUsage.TotalMicroSeconds / 1000d;
            double perCents = elapsed.TotalSeconds == 0d ? 0 : (user + kernel) / 1000d / elapsed.TotalSeconds;

            var elapsedFormatted = ElapsedFormatter.FormatElapsed(elapsed);

            var alreadyHasMilliseconds = elapsedFormatted.IndexOf("millisecond", StringComparison.OrdinalIgnoreCase) >= 0;
            var cpuUsageHumanized =
                hasCpuUsage
                    ? $" (cpu: {perCents * 100:f0}%, {user + kernel:n3} = {user:n3} [user] + {kernel:n3} [kernel]{(!alreadyHasMilliseconds ? " milliseconds" : "")})"
                    : null;

            var outcomeStatus = TestContext.CurrentContext.Result.Outcome.Status.ToString().ToUpper();
            if (AnsiSupportInfo.IsAnsiSupported)
            {
                char esc = (char)27;
                bool isOk = "PASSED".Equals(outcomeStatus, StringComparison.OrdinalIgnoreCase);
                string color = isOk ? "[92m" : "[31m";
                outcomeStatus = $"{esc}{color}{outcomeStatus}{esc}[0m";
            }

            Console.WriteLine($"← {stage.FormattedIndex} {stage.FixtureFullName}::{stage.TestName} >{outcomeStatus}< in {elapsedFormatted}{cpuUsageHumanized}");

        }

    }
}