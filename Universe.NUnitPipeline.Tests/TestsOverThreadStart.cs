extern alias nunit;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Threading;
using nunit::NUnit.Framework;
using Universe.NUnitPipeline;

namespace Tests
{
	[NUnitPipelineAction]
	[TestFixture]
	public class TestsOverThreadStart
	{
		[Test]
		[TestCase("First", 7)]
		[TestCase("Next", 200)]
		public void OverThreadStartSuccess(string title, [BeautyParameter] int milliseconds)
		{
			Console.WriteLine(PropertyBagVisualizer.ShowHumanString("SYNCHRONOUS"));
			Thread thread = new Thread(() => {
				Console.WriteLine(PropertyBagVisualizer.ShowHumanString("A-SYNCHRONOUS"));
				CpuLoad.RunSync(milliseconds);
				TestCleaner.OnDispose("Delete GLOBAL.TMP (from test body)", () => { }, TestDisposeOptions.Global);
			});
			thread.Start();
			thread.Join();
		}

		[Test]
		[Category("Fail")]
		[TestCase("First", 7)]
		[TestCase("Next", 200)]
		public void OverThreadStartException(string title, [BeautyParameter] int milliseconds)
		{
			Exception caught = null;
			Console.WriteLine(PropertyBagVisualizer.ShowHumanString("SYNCHRONOUS"));
			Thread thread = new Thread(() => {
				try
				{
					Console.WriteLine(PropertyBagVisualizer.ShowHumanString("A-SYNCHRONOUS"));
					CpuLoad.RunSync(milliseconds);
					TestCleaner.OnDispose("Delete GLOBAL.TMP (from test body)", () => { }, TestDisposeOptions.Global);
					throw new InvalidOperationException("Exception on purpose");
				}
				catch (Exception ex)
				{
					caught = ex;
				}
			});
			thread.Start();
			thread.Join();
			if (caught != null) throw caught;
		}

		[Test]
		[Category("Fail")]
		[TestCase("First", 7)]
		[TestCase("Next", 200)]
		public void OverThreadStartFail(string title, [BeautyParameter] int milliseconds)
		{
			Exception caught = null;
			Console.WriteLine(PropertyBagVisualizer.ShowHumanString("SYNCHRONOUS"));
			Thread thread = new Thread(() => {
				try
				{
					Console.WriteLine(PropertyBagVisualizer.ShowHumanString("A-SYNCHRONOUS"));
					CpuLoad.RunSync(milliseconds);
					TestCleaner.OnDispose("Delete GLOBAL.TMP (from test body)", () => { }, TestDisposeOptions.Global);
					Assert.Fail("Fail on purpose");
				}
				catch (Exception ex)
				{
					caught = ex;
				}
			});
			thread.Start();
			thread.Join();
			if (caught != null) throw caught;
		}
	}
}
