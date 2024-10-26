using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Universe.NUnitPipeline.Shared;

namespace Universe.NUnitPipeline
{
    [Flags]
    public enum TestDisposeOptions
    {
        Global = 1,
        Class = 2, // It needs [Action]-Attribute on each test fixture
        TestCase = 4,
        Async = 8,
        AsyncGlobal = Async | Global,
        AsyncClass = Async | Class,
        AsyncTestCase = Async | TestCase,
        IgnoreError = 16,
        // Async+Global Has No Sense?
    }

    public class TestCleaner
    {
        public static void OnDispose(string title, Action action, TestDisposeOptions modeWhen)
        {
            if (title.IndexOf('\'') < 0)
                title = $"'{title}'";
            else if (title.IndexOf("\"") < 0)
                title = $"\"{title}\"";

            // argument
            if (modeWhen == TestDisposeOptions.Async) modeWhen = TestDisposeOptions.AsyncGlobal;
            bool isIgnoringError = (modeWhen & TestDisposeOptions.IgnoreError) != 0;
            bool needGlobal = (modeWhen & TestDisposeOptions.Global) != 0;
            bool needClass = (modeWhen & TestDisposeOptions.Class) != 0;
            bool needTestCase = !needGlobal && !needClass;
            var needAsync = (modeWhen & TestDisposeOptions.Async) != 0;

            var testAdapter = TestContext.CurrentContext.Test;
            var testName = testAdapter.Name;
            bool isActualTest = !string.IsNullOrEmpty(testAdapter.MethodName); // false for onetimesetup/onetimeteardown
            if (!isActualTest && needTestCase)
            {
                needTestCase = false;
                needClass = true;
            }

            string collectionKey = needGlobal ? "Global" : $"Test {testAdapter.ClassName}::{testAdapter.Name}";
            if (needClass) collectionKey = $"Class {testAdapter.ClassName}";

			// testIndex is only for log
			// var tIndex = TestContext.CurrentContext.Test.GetProperty<TestCaseIndex>(NUnitTestCaseCounter.COUNTER_PROPERTY_NAME);
			// var testIndexFullName = tIndex == null ? "" : $"{tIndex.ClassIndex}.{tIndex.TestIndex} ";
			
			var indexState = TestContext.CurrentContext.Test.GetProperty<NUnitPipelineActionAttribute.IndexState>("Index State");
			var testIndexFullName = indexState == null ? "" :  $"{indexState} ";

            string isAsyncHumanized = $"{(needAsync ? " asynchronously" : "")}";
            string testFullName = $"{testAdapter.ClassName}::{testAdapter.Name}";
            if (!isActualTest) testFullName = $"{testAdapter.ClassName}";

            string prefix = $"Dispose for {testIndexFullName}{testFullName}{isAsyncHumanized}";
            if (needGlobal)
            {
                prefix = $"Dispose for {testIndexFullName}{testFullName}{isAsyncHumanized}";
            }
            else if (needClass)
            {
                prefix = $"Dispose for Class {testIndexFullName}{testFullName}{isAsyncHumanized}";
            }

            var letsDebug = "ok";
            Action actionWithLog = () =>
            {

                Stopwatch sw = Stopwatch.StartNew();
                try
                {
                    action();
                    var msec = sw.ElapsedTicks * 1000d / Stopwatch.Frequency;
                    OutputConsole.WriteLine($"[{prefix}] {title} success (took {msec:n1} milliseconds)");
                }
                catch (Exception ex)
                {
                    var msec = sw.ElapsedTicks * 1000d / Stopwatch.Frequency;
                    var err = isIgnoringError ? $". [{ex.GetType()}] {ex.Message}" : Environment.NewLine + ex;
                    OutputConsole.WriteLine($"[{prefix}] {title} failed (took {msec:n1} milliseconds){err}");
                }
            };

            var actionWrapped = actionWithLog;
            if (needAsync)
            {
	            OutputConsole.WriteLine($"[Test Cleaner] Postpone asynchronously {title} for KEY='{collectionKey}'");
                ManualResetEventSlim manualWait = new ManualResetEventSlim(false);
                AsyncDisposeWaiter.AddWaiter(manualWait.WaitHandle);
                actionWrapped = () =>
                {
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        try
                        {
                            actionWithLog();
                        }
                        finally
                        {
                            manualWait.Set();
                        }
                    });
                };
            }
            else
            {
	            OutputConsole.WriteLine($"[Test Cleaner] Enlisted dispose {title} for KEY='{collectionKey}'");
            }


            NUnitGlobalDisposeStorage.AddDisposeAction(collectionKey, actionWrapped);
        }
    }
}
