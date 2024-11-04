using NUnitLite;
using Tests;

namespace Universe.NUnitPipeline.Tests.Light
{
	internal class ProgramTestRunner
	{
		static int Main(string[] args)
		{
			return new AutoRun(typeof(TestsAsync).Assembly).Execute(args);
		}
	}
}
