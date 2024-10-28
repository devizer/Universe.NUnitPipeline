using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Universe.NUnitPipeline;
using Universe.NUnitPipeline.Tests;

namespace Tests
{
	[NUnitPipelineAction]
    [TestFixture]
    public class TestsAsync
    {
        [Test]
        [TestCase("First", 7)]
        [TestCase("Next", 200)]
		public async Task AsyncSuccess(string title, int milliseconds)
        {
            Console.WriteLine(PropertyBagVisualizer.ShowHumanString("SYNCHRONOUS"));
            await CpuLoad.RunAsync(milliseconds);
            TestCleaner.OnDispose("Delete GLOBAL.TMP (from test body)", () => File.Delete("Global.Temp"), TestDisposeOptions.Global);
        }

        [Test]
        [TestCase("First", 7)]
        [TestCase("Next", 200)]
        [Category("Fail")]
        public async Task AsyncFail(string title, int milliseconds)
        {
            await Task.Run(function: async () =>
            {
                TestCleaner.OnDispose("Delete File TempAsync.Temp (from test's body)", () => File.Delete("TempAsync.Temp"), TestDisposeOptions.TestCase);
                await CpuLoad.RunAsync(milliseconds);
                Assert.Fail("Fail on purpose");
            });
        }

        [Test]
        [TestCase("First", 7)]
        [TestCase("Next", 200)]
        [Category("Fail")]
        public async Task AsyncException(string title, int milliseconds)
        {
            await Task.Run(function: async () =>
            {
                await CpuLoad.RunAsync(milliseconds);
                throw new InvalidOperationException("Exception on purpose");
            });
        }
    }
}
