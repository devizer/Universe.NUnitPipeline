using System;
using NUnit.Framework.Interfaces;

namespace Universe.NUnitPipeline
{
    public class NUnitPipelineChainAction
    {
        public string Title { get; set; }
        public Action<NUnitStage,ITest> Action { get; set; }
    }
}