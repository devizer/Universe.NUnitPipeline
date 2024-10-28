using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Universe.NUnitPipeline;
using Universe.NUnitPipeline.Tests;

namespace Tests
{
	[NUnitPipelineAction]
	[TestFixture]
	public class TestsOverThreadPool
	{
		[Test]
		[TestCase("First", 7)]
		[TestCase("Next", 200)]
		public void SuccessOverThreadPool(string title, int milliseconds)
		{
			Console.WriteLine(PropertyBagVisualizer.ShowHumanString("SYNCHRONOUS"));
			ManualResetEventSlim waiter = new ManualResetEventSlim(false);
			ThreadPool.QueueUserWorkItem(_ =>
			{
				Console.WriteLine(PropertyBagVisualizer.ShowHumanString("A-SYNCHRONOUS"));
				CpuLoad.RunAsync(milliseconds).Wait();
				TestCleaner.OnDispose("Delete GLOBAL.TMP (from test body)", () => File.Delete("Global.Temp"), TestDisposeOptions.Global);
				waiter.Set();
			});
			waiter.Wait();
		}

		[Test]
		[TestCase("First", 7)]
		[TestCase("Next", 200)]
		public void FailOverThreadPoolFail(string title, int milliseconds)
		{
			Console.WriteLine(PropertyBagVisualizer.ShowHumanString("SYNCHRONOUS"));
			ManualResetEventSlim waiter = new ManualResetEventSlim(false);
			ThreadPool.QueueUserWorkItem(_ =>
			{
				try
				{
					Console.WriteLine(PropertyBagVisualizer.ShowHumanString("A-SYNCHRONOUS"));
					CpuLoad.RunAsync(milliseconds).Wait();
					TestCleaner.OnDispose("Delete GLOBAL.TMP (from test body)", () => File.Delete("Global.Temp"), TestDisposeOptions.Global);
					Assert.Fail("Fail on purpose");
				}
				finally
				{
					waiter.Set();
				}
			});
			waiter.Wait();
		}

		[Test]
		[TestCase("First", 7)]
		[TestCase("Next", 200)]
		public void ExceptionOverThreadPool(string title, int milliseconds)
		{
			Exception caught = null;
			Console.WriteLine(PropertyBagVisualizer.ShowHumanString("SYNCHRONOUS"));
			ManualResetEventSlim waiter = new ManualResetEventSlim(false);
			ThreadPool.QueueUserWorkItem(_ =>
			{
				try
				{
					Console.WriteLine(PropertyBagVisualizer.ShowHumanString("A-SYNCHRONOUS"));
					CpuLoad.RunAsync(milliseconds).Wait();
					TestCleaner.OnDispose("Delete GLOBAL.TMP (from test body)", () => File.Delete("Global.Temp"), TestDisposeOptions.Global);
					throw new InvalidOperationException("Exception on purpose");
				}
				catch(Exception ex)
				{
					caught = ex;
				}
				finally
				{
					waiter.Set();
				}
			});
			waiter.Wait();
			if (caught != null) throw caught;
		}




	}
}
