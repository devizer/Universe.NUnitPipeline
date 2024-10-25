﻿using System;
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

            if (modeWhen == TestDisposeOptions.Async) modeWhen = TestDisposeOptions.AsyncGlobal;

            bool isIgnoringError = (modeWhen & TestDisposeOptions.IgnoreError) != 0;
            bool isGlobal = (modeWhen & TestDisposeOptions.Global) != 0;
            bool isClass = (modeWhen & TestDisposeOptions.Class) != 0;
            bool isTestCase = !isGlobal && !isClass;
            var isAsync = (modeWhen & TestDisposeOptions.Async) != 0;

            var testAdapter = TestContext.CurrentContext.Test;
            var testName = testAdapter.Name;
            bool isActualTest = !string.IsNullOrEmpty(testAdapter.MethodName); // false for onetimesetup/onetimeteardown
            if (!isActualTest && isTestCase)
            {
                isTestCase = false;
                isGlobal = true;
            }

            string collectionKey = isGlobal ? "Global" : $"Test Case {testAdapter.ClassName}::{testAdapter.Name}";
            if (isTestCase) collectionKey = $"Class {testAdapter.ClassName}";

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
            else if (isClass)
            {
                prefix = $"Dispose Class {testIndexFullName}{testFullName}{isAsyncHumanized}";
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

            NUnitGlobalDisposeStorage.AddDisposeAction(collectionKey, actionWrapped);
        }
    }
}
