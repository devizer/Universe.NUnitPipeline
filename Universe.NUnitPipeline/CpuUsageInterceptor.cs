extern alias nunit;
using nunit::NUnit.Framework.Interfaces;
using nunit::NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Universe.CpuUsage;
using Universe.NUnitPipeline.ConsoleTreeTable;

namespace Universe.NUnitPipeline
{
	extern alias nunit;

	public class CpuUsageInterceptor
    {
        public static readonly string Title = "Cpu Usage";
        private static readonly object Sync = new object();

        class CpuUsageState
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
                if (null == test.GetPropertyOrAdd<CpuUsageState>(nameof(CpuUsageState), null))
                {
                    var cpuUsageTestState = new CpuUsageState()
                    {
                        Stopwatch = Stopwatch.StartNew(),
                        ThreadIdOnStart = Thread.CurrentThread.ManagedThreadId,
                        CpuUsageOnStart = CpuUsage.CpuUsage.GetByThread(),
                        CpuUsageAsyncWatcher = new CpuUsageAsyncWatcher(),
                    };
                    var _ = test.GetPropertyOrAdd<CpuUsageState>(nameof(CpuUsageState), t => cpuUsageTestState);
                }
            }
        }

        public static void OnFinish(NUnitStage stage, ITest test)
        {
            if (stage.NUnitActionAppliedTo != NUnitActionAppliedTo.Test) return;

            CpuUsageState cpuUsageState;

            lock (Sync)
            {
                cpuUsageState = test.GetPropertyOrAdd<CpuUsageState>(nameof(CpuUsageState), null);
            }
            if (cpuUsageState == null || cpuUsageState.Finished) return;
            
            var elapsed = cpuUsageState.Stopwatch.Elapsed;
            cpuUsageState.CpuUsageAsyncWatcher.Stop();
            cpuUsageState.Finished = true;

            var asyncTotals = cpuUsageState.CpuUsageAsyncWatcher.Totals;
            CpuUsage.CpuUsage? syncCpuUsage = null;
            if (cpuUsageState.CpuUsageOnStart.HasValue && Thread.CurrentThread.ManagedThreadId == cpuUsageState.ThreadIdOnStart)
            {
                var cpuUsageAtEnd = CpuUsage.CpuUsage.GetByThread();
                if (cpuUsageAtEnd.HasValue)
                {
                    syncCpuUsage = cpuUsageAtEnd.Value - cpuUsageState.CpuUsageOnStart.Value;
                }
            }

            bool hasCpuUsage = syncCpuUsage.HasValue || asyncTotals.Count > 0;
            var finalCpuUsage = asyncTotals.GetSummaryCpuUsage() + syncCpuUsage.GetValueOrDefault();
            
            CpuUsageResult cpuUsageResult = new CpuUsageResult() { Elapsed = elapsed, CpuUsage = hasCpuUsage ? finalCpuUsage : (CpuUsage.CpuUsage?)null };
            test.GetPropertyOrAdd(nameof(CpuUsageResult), t => cpuUsageResult );

        }
    }
}
