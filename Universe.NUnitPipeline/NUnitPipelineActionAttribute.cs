﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Universe.CpuUsage;
using Universe.NUnitPipeline;

/*
 * Supported NUnit (and Console Runner): 3.12 ... 3.18.3 and 4x
 */
namespace Universe {

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
                Console.WriteLine($"[DEBUG Action:OnStart] Invoke Action '{a.Title}'");
                a.Action(stage);
            }
        }


        public void AfterTest(ITest test)
        {
            BuildNUnitStage(test, NUnitActionSide.Finish, out var stage, out var counter);
            bool isFirst = counter == 1;
            if (counter != 1) return;

            var actions = NUnitPipelineChain.OnStart;
            if (actions == null) return;
            foreach (var a in actions)
            {
                Console.WriteLine($"[DEBUG Pipeline.Action:OnFinish] Invoke Action '{a.Title}'");
                a.Action(stage);
            }
        }

        class SelfCounter
        {
            public int Count = 0;
        }

        void BuildNUnitStage(ITest test, NUnitActionSide actionSide, out NUnitStage stage, out int counter)
        {
            var selfCounter = test.GetPropertyOrAdd<SelfCounter>($"Is First on {actionSide}", t => new SelfCounter() { Count = 0 });
            selfCounter.Count++;
            counter = selfCounter.Count;
            if (counter == 1 && actionSide == NUnitActionSide.Start) Console.WriteLine(EmptyLineBetweenTests());

            stage = new NUnitStage() { Side = actionSide, ITest = test };
            if (test.Fixture == null)
            {
                stage.NUnitActionAppliedTo = NUnitActionAppliedTo.Assembly;
                var assemblyFileName = string.IsNullOrEmpty(test.FullName) ? null : Path.GetFileName(test.FullName);
                stage.StructuredFullName = new[] { assemblyFileName ?? "A Test Assembly" };
            }
            else
            {
                stage.FixtureFullName = test.Fixture.GetType().FullName;
                var testParts = new string[0];
                if (test.Method == null)
                {
                    stage.NUnitActionAppliedTo = NUnitActionAppliedTo.Fixture;
                }
                else
                {
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
            }

            Console.WriteLine($"[DEBUG Action:On{actionSide}] STAGE {stage.Side} '{stage.NUnitActionAppliedTo} Counter={counter}': {stage.FormattedIndex} [{string.Join(", ", stage.StructuredFullName)}]");
        }


        static string EmptyLineBetweenTests()
        {
#if NETFRAMEWORK
            return Environment.NewLine;
#else
            return string.Empty;
#endif

        }



    }
}