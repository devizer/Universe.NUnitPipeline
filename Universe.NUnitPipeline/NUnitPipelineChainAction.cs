using System;

namespace Universe.NUnitPipeline
{
    public class NUnitPipelineChainAction
    {
        public string Title { get; set; }
        public Action<NUnitStage> Action { get; set; }
    }
}