using System;
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

        class SelfCounter
        {
            public int Count = 0;
        }
        public void BeforeTest(ITest test)
        {
            var selfCounter = test.GetPropertyOrAdd<SelfCounter>("Is First", t => new SelfCounter() { Count = 0});
            selfCounter.Count++;
            bool isNext = selfCounter.Count > 1;
            if (!isNext) Console.WriteLine("");

            NUnitStage stage = new NUnitStage() { Side = NUnitActionSide.Start, };
            if (test.Fixture == null)
            {
                stage.NUnitActionAppliedTo = NUnitActionAppliedTo.Assembly;
                var assemblyFileName = string.IsNullOrEmpty(test.FullName) ? null : Path.GetFileName(test.FullName);
                stage.StructuredFullName = new[] { assemblyFileName ?? "A Test Assembly" };
            }
            else
            {
                stage.FixtureFullName = NUnitActionAppliedTo.Fixture.GetType().FullName;
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

            Console.WriteLine($"STAGE {stage.Side} '{stage.NUnitActionAppliedTo} Counter={selfCounter.Count}': [{string.Join(", ", stage.StructuredFullName)}]");
        }


        public void AfterTest(ITest test)
        {
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