extern alias nunit;
using System.Collections.Generic;
using System.IO;
using nunit::NUnit.Framework;
using Universe.NUnitPipeline;

[assembly: NUnitPipelineAction]

namespace Tests
{
	// Single shared configuration for each test assemblies
	public class OrderedPipelineConfiguration
	{
		public static void Configure()
		{
			NUnitPipelineChain.InternalReportFile = Path.Combine("TestsOutput", $"Flow ({TargetFrameworkInfo.ShortName})");

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
				new NUnitPipelineChainAction() { Title = CpuUsageTreeReportInterceptor.Title, Action = CpuUsageTreeReportInterceptor.OnFinish },
			};
		}

	}
    
	// One time setup fixture per each test project
	[SetUpFixture]
    public class OrderedPipelineConfigurationFixture 
    {
        [OneTimeSetUp]
        public void Configure()
        {
			OrderedPipelineConfiguration.Configure();
        }
    }
}
