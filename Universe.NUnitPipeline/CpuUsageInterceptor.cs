using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Universe.CpuUsage;

namespace Universe.NUnitPipeline
{
    public class CpuUsageInterceptor
    {
        public static readonly string Title = "Cpu Usage";
        private static readonly object Sync = new object();

        class CpuUsageTestState
        {
            public Stopwatch Stopwatch { get; set; }
            public CpuUsageAsyncWatcher CpuUsageAsyncWatcher { get; set; }
            public CpuUsage.CpuUsage? CpuUsageOnStart { get; set; }
            public int ThreadIdOnStart { get; set; }
            public bool Finished { get; set; } = false;

            public override string ToString()
            {
                return $"{nameof(Finished)}: {Finished}";
            }
        }



        public static void OnStart(NUnitStage stage, ITest test)
        {
            if (stage.NUnitActionAppliedTo != NUnitActionAppliedTo.Test) return;

            lock (Sync)
            {
                if (null == test.GetPropertyOrAdd<CpuUsageTestState>(nameof(CpuUsageTestState), null))
                {
                    var cpuUsageTestState = new CpuUsageTestState()
                    {
                        Stopwatch = Stopwatch.StartNew(),
                        ThreadIdOnStart = Thread.CurrentThread.ManagedThreadId,
                        CpuUsageOnStart = CpuUsage.CpuUsage.GetByThread(),
                        CpuUsageAsyncWatcher = new CpuUsageAsyncWatcher(),
                    };
                    var _ = test.GetPropertyOrAdd<CpuUsageTestState>(nameof(CpuUsageTestState), t => cpuUsageTestState);
                }
            }
        }

        public static void OnFinish(NUnitStage stage, ITest test)
        {
            if (stage.NUnitActionAppliedTo != NUnitActionAppliedTo.Test) return;

            CpuUsageTestState cpuUsageTestState;

            lock (Sync)
            {
                cpuUsageTestState = test.GetPropertyOrAdd<CpuUsageTestState>(nameof(CpuUsageTestState), null);
            }
            if (cpuUsageTestState == null || cpuUsageTestState.Finished) return;
            
            var elapsed = cpuUsageTestState.Stopwatch.Elapsed;
            cpuUsageTestState.CpuUsageAsyncWatcher.Stop();
            cpuUsageTestState.Finished = true;

            var asyncTotals = cpuUsageTestState.CpuUsageAsyncWatcher.Totals;
            CpuUsage.CpuUsage? syncCpuUsage = null;
            if (cpuUsageTestState.CpuUsageOnStart.HasValue && Thread.CurrentThread.ManagedThreadId == cpuUsageTestState.ThreadIdOnStart)
            {
                var cpuUsageAtEnd = CpuUsage.CpuUsage.GetByThread();
                if (cpuUsageAtEnd.HasValue)
                {
                    syncCpuUsage = cpuUsageAtEnd.Value - cpuUsageTestState.CpuUsageOnStart.Value;
                }
            }

            bool hasCpuUsage = syncCpuUsage.HasValue || asyncTotals.Count > 0;
            var finalCpuUsage = asyncTotals.GetSummaryCpuUsage() + syncCpuUsage.GetValueOrDefault();
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

            Console.WriteLine($"← {stage.FormattedIndex} {stage.FixtureFullName}::{stage.TestName} >{outcomeStatus}< in {elapsedFormatted}{cpuUsageHumanized}");

        }
    }
}
