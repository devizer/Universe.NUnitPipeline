extern alias nunit;
using System;
using System.Collections.Generic;
using System.IO;
using nunit::NUnit.Framework;
using Universe.NUnitPipeline;

namespace Tests
{
	[NUnitPipelineAction]
	[TestFixture]
	public class FormatElapsedTests
	{
		[Test]
		public void FormatElapsedTest()
		{
			Console.WriteLine(PropertyBagVisualizer.ShowHumanString("SYNCHRONOUS"));
			List<TimeSpan> array = new List<TimeSpan>
			{
				TimeSpan.FromSeconds(7 * 24 * 3600 + 15 * 3600 + 1234.5678d),
				TimeSpan.FromSeconds(15 * 3600 + 1234.5678d),
				TimeSpan.FromSeconds(1 * 3600 + 1234.5678d),
				TimeSpan.FromSeconds(50 * 60 + 123),
				TimeSpan.FromSeconds(10 * 60 + 123),
				TimeSpan.FromSeconds(123.4),
				TimeSpan.FromSeconds(2.345),
				TimeSpan.FromSeconds(0.123),
				TimeSpan.FromSeconds(0.0123),
			};
			foreach (var timeSpan in array)
			{
				Console.WriteLine($"TIMESPAN {timeSpan} --> \"{ElapsedFormatter.FormatElapsed(timeSpan)}\"");
			}

			TestCleaner.OnDispose("ASYNC Delete File AsyncTemporary.Temp (from test body)", () => { }, TestDisposeOptions.AsyncTestCase);
			TestCleaner.OnDispose("Delete File GlobalAsyncTemporary.Temp (from test body)", () => File.Delete("GlobalAsyncTemporary.Temp"), TestDisposeOptions.AsyncGlobal);
		}

	}
}
