extern alias nunit;
using nunit::NUnit.Framework.Interfaces;

using System;
using System.Collections.Generic;
using System.Diagnostics;



namespace Universe.NUnitPipeline
{
	public class DisposeInterceptor
    {
        public static readonly string Title = "Dispose";

        public static void OnFinish(NUnitStage stage, ITest test)
        {

            string collectionKey = null;
            if (stage.NUnitActionAppliedTo == NUnitActionAppliedTo.Assembly)
            {
                collectionKey = "Global";
            }
            else if (stage.NUnitActionAppliedTo == NUnitActionAppliedTo.Fixture)
            {
                collectionKey = $"Class {test.ClassName}";
            }
            else if (stage.NUnitActionAppliedTo == NUnitActionAppliedTo.Test)
            {
                collectionKey = $"Test {test.ClassName}::{test.Name}";
            }

            bool isGlobalDispose = stage.NUnitActionAppliedTo == NUnitActionAppliedTo.Assembly;

            if (collectionKey == null)
            {
                Console.WriteLine("[Dispose] Warning! Unknown context. Please Report");
                return;
            }

            List<Action> actionList = NUnitGlobalDisposeStorage.FetchDisposeActions(collectionKey);
            PipelineLog.LogTrace($"[DISPOSE] Starting Dispose '{collectionKey}'. Actions: \"{actionList?.Count}\"");

            if (actionList != null && actionList.Count > 0)
            {
                if (isGlobalDispose) PipelineLog.WriteLine($"{Environment.NewLine}[Dispose] Starting Global Dispose");
                Stopwatch sw = Stopwatch.StartNew();
                foreach (var action in actionList) action();
                var msec = sw.ElapsedTicks * 1000d / Stopwatch.Frequency;
                if (isGlobalDispose)
	                PipelineLog.WriteLine($"[Dispose {collectionKey}] Completed in {msec:n1} milliseconds");
            }

            if (isGlobalDispose)
            {
                var countAsyncWaiters = AsyncDisposeWaiter.GetCount();
                if (countAsyncWaiters.Total > 0)
                {
                    if (countAsyncWaiters.Incomplete > 0)
                    {
	                    PipelineLog.WriteLine($"[Global Dispose] Waiting for {countAsyncWaiters.Incomplete} incomplete async cleaner(s). Already completed: {countAsyncWaiters.Completed}");
                        AsyncDisposeWaiter.WaitAll();
                    }

                    PipelineLog.WriteLine($"[Global Dispose] All the {countAsyncWaiters.Total} async cleaner(s) is (are) finished");
                }
            }

        }
    }
}
