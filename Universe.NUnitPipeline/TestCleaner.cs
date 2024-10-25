using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Universe.NUnitPipeline
{
    [Flags]
    public enum TestDisposeOptions
    {
        Global = 1,
        Class = 2, // It needs [Action]-Attribute on each test fixture
        TestCase = 4,
        Async = 8,
        IgnoreError = 16,
        // Async+Global Has No Sense?
    }

    public class TestCleaner
    {
        public static void OnDispose(string title, Action action, TestDisposeOptions mode)
        {
            if (title.IndexOf('\'') < 0)
                title = $"'{title}'";
            else if (title.IndexOf("\"") < 0)
                title = $"\"{title}\"";

            bool isIgnoringError = (mode & TestDisposeOptions.IgnoreError) != 0;
            bool isGlobal = (mode & TestDisposeOptions.Global) != 0;
            bool isTestCase = !isGlobal;
            var isAsync = (mode & TestDisposeOptions.Async) != 0;

            var testAdapter = TestContext.CurrentContext.Test;
            var testName = testAdapter.Name;
            bool isActualTest = !string.IsNullOrWhiteSpace(testAdapter.MethodName); // false for onetimesetup/onetimeteardown
            if (!isActualTest && isTestCase)
            {
                isTestCase = false;
                isGlobal = true;
            }

            string collectionKey = isGlobal ? "Global" : $"Test Case {testAdapter.ClassName}::{testAdapter.Name}";

            // testIndex is only for log
            // var tIndex = TestContext.CurrentContext.Test.GetProperty<TestCaseIndex>(NUnitTestCaseCounter.COUNTER_PROPERTY_NAME);
            // var testIndexFullName = tIndex == null ? "" : $"{tIndex.ClassIndex}.{tIndex.TestIndex} ";
            var testIndexFullName = "";
            string isAsyncHumanized = $"{(isAsync ? " asynchronously" : "")}";
            string testFullName = $"{testAdapter.ClassName}::{testAdapter.Name}";
            if (!isActualTest) testFullName = $"{testAdapter.ClassName}";

            string prefix = $"Dispose {testIndexFullName}{testFullName}{isAsyncHumanized}";
            if (isGlobal)
            {
                prefix = $"Dispose Global {testIndexFullName}{testFullName}{isAsyncHumanized}";
            }

            var letsDebug = "ok";
            Action actionWithLog = () =>
            {

                Stopwatch sw = Stopwatch.StartNew();
                try
                {
                    action();
                    var msec = sw.ElapsedTicks * 1000d / Stopwatch.Frequency;
                    Console.WriteLine($"[{prefix}] {title} success (took {msec:n1} milliseconds)");
                }
                catch (Exception ex)
                {
                    var msec = sw.ElapsedTicks * 1000d / Stopwatch.Frequency;
                    var err = isIgnoringError ? $". [{ex.GetType()}] {ex.Message}" : Environment.NewLine + ex;
                    Console.WriteLine($"[{prefix}] {title} failed (took {msec:n1} milliseconds){err}");
                }
            };

            var actionWrapped = actionWithLog;
            if (isAsync)
            {
                Console.WriteLine($"Postpone {title} for {prefix}");
                ManualResetEventSlim manualWait = new ManualResetEventSlim(false);
                AsyncDisposeWaiter.AddWaiter(manualWait.WaitHandle);
                actionWrapped = () =>
                {
                    Task.Run(() =>
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

            NUnitGlobalDisposeStorage.AddDisposeAction(collectionKey, actionWrapped);
        }
    }
}
