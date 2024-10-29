extern alias nunit;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using nunit::NUnit.Framework;
using Universe.NUnitPipeline;
using Universe.NUnitPipeline.Tests;

namespace Tests
{
	[NUnitPipelineAction]
	[TestFixture]
	public class TestsOverThreadPool
	{
		[Test]
		[Category("Fail")]
		[TestCase("First", 7)]
		[TestCase("Next", 200)]
		public void SuccessOverThreadPool(string title, [BeautyParameter] int milliseconds)
		{
			Console.WriteLine(PropertyBagVisualizer.ShowHumanString("SYNCHRONOUS"));
			ManualResetEventSlim waiter = new ManualResetEventSlim(false);
			ThreadPool.QueueUserWorkItem(_ =>
			{
				Console.WriteLine(PropertyBagVisualizer.ShowHumanString("A-SYNCHRONOUS"));
				CpuLoad.RunSync(milliseconds);
				TestCleaner.OnDispose("Delete GLOBAL.TMP (from test body)", () => File.Delete("Global.Temp"), TestDisposeOptions.Global);
				waiter.Set();
			});
			waiter.Wait();
		}

		[Test]
		[Category("Fail")]
		[TestCase("First", 7)]
		[TestCase("Next", 200)]
		public void FailOverThreadPoolFail(string title, [BeautyParameter] int milliseconds)
		{
			Exception caught = null;
			Console.WriteLine(PropertyBagVisualizer.ShowHumanString("SYNCHRONOUS"));
			ManualResetEventSlim waiter = new ManualResetEventSlim(false);
			ThreadPool.QueueUserWorkItem(_ =>
			{
				try
				{
					Console.WriteLine(PropertyBagVisualizer.ShowHumanString("A-SYNCHRONOUS"));
					CpuLoad.RunSync(milliseconds);
					TestCleaner.OnDispose("Delete GLOBAL.TMP (from test body)", () => File.Delete("Global.Temp"), TestDisposeOptions.Global);
					Assert.Fail("Fail on purpose");
				}
				// Without catch nunit3-console works properly, but dotnet test and vs runner fail
				catch (Exception ex)
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

		[Test]
		[Category("Fail")]
		[TestCase("First", 7)]
		[TestCase("Next", 200)]
		public void ExceptionOverThreadPool(string title, [BeautyParameter] int milliseconds)
		{
			Exception caught = null;
			Console.WriteLine(PropertyBagVisualizer.ShowHumanString("SYNCHRONOUS"));
			ManualResetEventSlim waiter = new ManualResetEventSlim(false);
			ThreadPool.QueueUserWorkItem(_ =>
			{
				try
				{
					Console.WriteLine(PropertyBagVisualizer.ShowHumanString("A-SYNCHRONOUS"));
					CpuLoad.RunSync(milliseconds);
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
