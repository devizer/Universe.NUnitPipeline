using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Universe.NUnitPipeline;
using Universe.NUnitPipeline.Tests;

namespace Tests
{
#if !NET35 && !NET40 && !NET45
	[NUnitPipelineAction]
    [TestFixture]
    public class TestsAsync
    {
        [Test]
        [TestCase("First", 7)]
        [TestCase("Next", 200)]
		public async Task AsyncSuccess(string title, [BeautyParameter] int milliseconds)
        {
            Console.WriteLine(PropertyBagVisualizer.ShowHumanString("SYNCHRONOUS"));
            await CpuLoad.RunAsync(milliseconds);
            TestCleaner.OnDispose("Delete GLOBAL.TMP (from test body)", () => File.Delete("Global.Temp"), TestDisposeOptions.Global);
        }

        [Test]
        [Category("Fail")]
        [TestCase("First", 7)]
        [TestCase("Next", 200)]
        public async Task AsyncFail(string title, [BeautyParameter] int milliseconds)
        {
            await Task.Run(function: async () =>
            {
                TestCleaner.OnDispose("Delete File TempAsync.Temp (from test's body)", () => File.Delete("TempAsync.Temp"), TestDisposeOptions.TestCase);
                await CpuLoad.RunAsync(milliseconds);
                Assert.Fail("Fail on purpose");
            });
        }

        [Test]
        [Category("Fail")]
        [TestCase("First", 7)]
        [TestCase("Next", 200)]
        public async Task AsyncException(string title, [BeautyParameter] int milliseconds)
        {
            await Task.Run(function: async () =>
            {
                await CpuLoad.RunAsync(milliseconds);
                throw new InvalidOperationException("Exception on purpose");
            });
        }
    }
#endif
}
