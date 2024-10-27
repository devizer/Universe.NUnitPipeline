using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Universe.NUnitPipeline.Shared;

/*
 * Supported NUnit (and Console Runner): 3.12 ... 3.18.3 and 4x
 */
namespace Universe.NUnitPipeline
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Assembly, AllowMultiple = false)]
    public class NUnitPipelineActionAttribute : Attribute, ITestAction
    {
        public ActionTargets Targets => ActionTargets.Test | ActionTargets.Suite;

        public void BeforeTest(ITest test)
        {
            BuildNUnitStage(test, NUnitActionSide.Start, out var stage, out var counter);
            if (counter != 1) return;

            var actions = NUnitPipelineChain.OnStart;
            if (actions == null) return;
            foreach (var a in actions)
            {
                DebugConsole.WriteLine($"[DEBUG Pipeline.Action:OnStart] Invoke START Action '{a.Title}' for `{stage.InternalKey}` ({stage.NUnitActionAppliedTo})");
                a.Action(stage, test);
            }
        }


        public void AfterTest(ITest test)
        {
            BuildNUnitStage(test, NUnitActionSide.Finish, out var stage, out var counter);
            bool isFirst = counter == 1;
            if (counter != 1) return;

            var actions = NUnitPipelineChain.OnEnd;
            if (actions == null) return;
            foreach (var a in actions)
            {
                DebugConsole.WriteLine($"[DEBUG Pipeline.Action:OnFinish] Invoke FINISH Action '{a.Title}' for `{stage.InternalKey}` ({stage.NUnitActionAppliedTo})");
                a.Action(stage, test);
            }

            if (stage.NUnitActionAppliedTo == NUnitActionAppliedTo.Assembly)
            {
	            var files = new[] 
	            {
		            new { Content = InternalLog.InternalBuffer.ToString(), Path = NUnitPipelineChain.InternalReportFile + ".Internal.Log" },
		            new { Content = InternalLog.Buffer.ToString(), Path = NUnitPipelineChain.InternalReportFile + ".Log" }
	            };

	            foreach (var file in files)
	            {
		            var name = Path.GetFullPath(file.Path);
		            TryAndForget.Execute(() => Directory.CreateDirectory(Path.GetDirectoryName(name)));
					Debug.WriteLine($"Internal Log: {name}");
					using(FileStream fs = new FileStream(name, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
					using (StreamWriter wr = new StreamWriter(fs, new UTF8Encoding(false)))
					{
						wr.Write(file.Content);
					}
	            }
            }
        }

        internal class PrivateCounter
        {
            public int Count = 0;

            public override string ToString()
            {
                return $"{Count:0}";
            }
        }

        void BuildNUnitStage(ITest test, NUnitActionSide actionSide, out NUnitStage stage, out int counter)
        {
            var selfCounter = test.GetPropertyOrAdd<PrivateCounter>($"Is First on {actionSide}", t => new PrivateCounter() { Count = 0 });
            selfCounter.Count++;
            counter = selfCounter.Count;
            // if (counter == 1 && actionSide == NUnitActionSide.Start) Console.WriteLine(EmptyLineBetweenTests());

            stage = new NUnitStage() { Side = actionSide };
            if (test.Fixture == null)
            {
                stage.NUnitActionAppliedTo = NUnitActionAppliedTo.Assembly;
                var assemblyFileName = string.IsNullOrEmpty(test.FullName) ? null : Path.GetFileName(test.FullName);
                stage.StructuredFullName = new[] { assemblyFileName ?? "A Test Assembly" };
                stage.InternalKey = "Global";
            }
            else
            {
                stage.FixtureFullName = test.Fixture.GetType().FullName;
                var testParts = new string[0];
                if (test.Method == null)
                {
                    stage.NUnitActionAppliedTo = NUnitActionAppliedTo.Fixture;
                    stage.InternalKey = $"Class {test.Fixture.GetType().FullName}";
                }
                else
                {
                    stage.InternalKey = $"Test {test.Fixture.GetType().FullName}::{test.Name}";
                    stage.NUnitActionAppliedTo = NUnitActionAppliedTo.Test;
                    stage.TestName = test.Name;
                    var pBracket = stage.TestName?.IndexOf("(");
                    if (pBracket.HasValue && pBracket > 0 && pBracket.Value < stage.TestName.Length - 1)
                        testParts = new[] { stage.TestName.Substring(0, pBracket.Value), stage.TestName };
                    else
                        testParts = new[] { stage.TestName };
                }


                stage.StructuredFullName = stage.FixtureFullName.Split('.')
                    .Union(testParts)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToArray();
            }

            if (stage.Side == NUnitActionSide.Start)
            {
                if (stage.NUnitActionAppliedTo == NUnitActionAppliedTo.Test)
                {
                    var testCaseIndex = test.GetTestCaseIndex();
                    stage.FixtureIndex = testCaseIndex.ClassIndex;
                    stage.TestIndex = testCaseIndex.TestIndex;
                }
                else if (stage.NUnitActionAppliedTo == NUnitActionAppliedTo.Fixture)
                {
                    var testCaseIndex = test.GetTestCaseIndex();
                    stage.FixtureIndex = testCaseIndex.ClassIndex;
                    stage.TestIndex = null;
                }
                else if (stage.NUnitActionAppliedTo == NUnitActionAppliedTo.Assembly)
                {
                }

                IndexState indexState = new IndexState() { FixtureIndex = stage.FixtureIndex, TestIndex = stage.TestIndex };
                var _ = test.GetPropertyOrAdd("Index State", t => indexState);
            }
            else if (stage.Side == NUnitActionSide.Finish)
            {
                var indexState = test.GetPropertyOrAdd<IndexState>("Index State", null);
                stage.FixtureIndex = indexState.FixtureIndex;
                stage.TestIndex = indexState.TestIndex;
            }

            DebugConsole.WriteLine($"[DEBUG Action:On{actionSide}] STAGE {stage.Side} '{stage.NUnitActionAppliedTo} Counter={counter}': {stage.FormattedIndex} [{string.Join(", ", stage.StructuredFullName)}]");
        }

        internal class IndexState
        {
            public int? FixtureIndex { get; set; }
            public int? TestIndex { get; set; } // If applied to test only

            public override string ToString()
            {
                return FixtureIndex.HasValue ? TestIndex.HasValue ? $"{FixtureIndex.Value}.{TestIndex.Value}" : $"{FixtureIndex.Value}" : "";
            }
        }


        static string EmptyLineBetweenTests()
        {
#if NETFRAMEWORK && false
            return Environment.NewLine;
#else
            return string.Empty;
#endif

        }



    }
}
