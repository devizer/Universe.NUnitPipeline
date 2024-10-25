using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Universe.NUnitPipeline
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Assembly, AllowMultiple = true)]
    public class NUnitPipelineAttribute : Attribute, ITestAction
    {
        public void BeforeTest(ITest test)
        {
            var actions = NUnitPipelineChain.OnStart;
            if (actions == null) return;
            foreach (var a in actions)
            {
                Console.WriteLine($"[DEBUG NUnitPipelineChain] OnStart Action '{a.Title}'");
            }
        }

        public void AfterTest(ITest test)
        {
            var actions = NUnitPipelineChain.OnEnd;
            if (actions == null) return;
            foreach (var a in actions)
            {
                Console.WriteLine($"[DEBUG NUnitPipelineChain] OnEnd Action '{a.Title}'");
            }
        }

        public ActionTargets Targets { get; } = ActionTargets.Suite | ActionTargets.Test;
    }
}