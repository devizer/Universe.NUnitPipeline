using System.Collections.Generic;

namespace Universe.NUnitPipeline
{
	public class NUnitPipelineChain
    {
        List<NUnitPipelineChainAction> _OnStart;
        List<NUnitPipelineChainAction> _OnEnd;


		private static readonly object SyncStart = new object(), SyncEnd = new object();

        public List<NUnitPipelineChainAction> OnStart
        {
            get
            {
                lock (SyncStart) return _OnStart;
            }
            set { lock (SyncStart) _OnStart = value; }
        }

        public List<NUnitPipelineChainAction> OnEnd
        {
            get
            {
                lock (SyncEnd) return _OnEnd;
            }
            set { lock (SyncEnd) _OnEnd = value; }
        }
    }
}
