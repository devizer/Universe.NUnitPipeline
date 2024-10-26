using System.Collections.Generic;

namespace Universe.NUnitPipeline
{
    public static class NUnitPipelineChain
    {
        static List<NUnitPipelineChainAction> _OnStart;
        static List<NUnitPipelineChainAction> _OnEnd;

        public static string InternalReportFile { get; set; }


		private static readonly object SyncStart = new object(), SyncEnd = new object();

        public static List<NUnitPipelineChainAction> OnStart
        {
            get
            {
                lock (SyncStart) return _OnStart;
            }
            set { lock (SyncStart) _OnStart = value; }
        }

        public static List<NUnitPipelineChainAction> OnEnd
        {
            get
            {
                lock (SyncEnd) return _OnEnd;
            }
            set { lock (SyncEnd) _OnEnd = value; }
        }
    }
}
