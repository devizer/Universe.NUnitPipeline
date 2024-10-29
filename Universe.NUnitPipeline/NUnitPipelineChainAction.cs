extern alias nunit;
using nunit::NUnit.Framework.Interfaces;
using nunit::NUnit.Framework;
using System;

namespace Universe.NUnitPipeline
{
    public class NUnitPipelineChainAction
    {
        public string Title { get; set; }
        public Action<NUnitStage,ITest> Action { get; set; }
    }
}
