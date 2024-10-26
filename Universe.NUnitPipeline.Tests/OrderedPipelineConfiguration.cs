using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Universe.NUnitPipeline.Tests;

namespace Universe.NUnitPipeline.Tests
{
    [SetUpFixture]
    public class OrderedPipelineConfiguration 
    {
        [OneTimeSetUp]
        public void Configure()
        {
            NUnitPipelineChain.OnStart = new List<NUnitPipelineChainAction>()
            {
                new NUnitPipelineChainAction() { Title = CpuUsageInterceptor.Title, Action = CpuUsageInterceptor.OnStart },
                new NUnitPipelineChainAction() { Title = CpuUsageVizInterceptor.Title, Action = CpuUsageVizInterceptor.OnStart },
            };

            NUnitPipelineChain.OnEnd = new List<NUnitPipelineChainAction>()
            {
                new NUnitPipelineChainAction() { Title = CpuUsageInterceptor.Title, Action = CpuUsageInterceptor.OnFinish },
                new NUnitPipelineChainAction() { Title = CpuUsageVizInterceptor.Title, Action = CpuUsageVizInterceptor.OnFinish },
                new NUnitPipelineChainAction() { Title = DisposeInterceptor.Title, Action = DisposeInterceptor.OnFinish },
            };
        }

    }
}
