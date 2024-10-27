using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Universe.NUnitPipeline.Tests
{

    [NUnitPipelineAction]
    [TestFixture]
    public class TestsAsync
    {
        [Test]
        [TestCase("First")]
        /*[TestCase("Next")]*/
        public async Task Test1Async_111_Milliseconds(string title)
        {
            Console.WriteLine(PropertyBagVisualizer.ShowHumanString("SYNCHRONOUS"));
            await CpuLoad.RunAsync(111);
            TestCleaner.OnDispose("Delete GLOBAL.TMP (from test body)", () => File.Delete("Global.Temp"), TestDisposeOptions.Global);
        }

        /*[Test]*/
        [TestCase("First")]
        [TestCase("Next")]
        public async Task Test2Async_777_Milliseconds(string title)
        {
            Console.WriteLine(PropertyBagVisualizer.ShowHumanString("SYNCHRONOUS"));
            await CpuLoad.RunAsync(777);
        }

        [Test]
        [Category("Fail")]
        public async Task AsyncFail()
        {
            await Task.Run(function: async () =>
            {
                TestCleaner.OnDispose("Delete File TempAsync.Temp (from test's body)", () => File.Delete("TempAsync.Temp"), TestDisposeOptions.TestCase);
                await CpuLoad.RunAsync(42);
                Assert.Fail("Fail on purpose");
            });
        }

        [Test]
        [Category("Fail")]
        public async Task AsyncException()
        {
            await Task.Run(function: async () =>
            {
                await CpuLoad.RunAsync(42);
                throw new InvalidOperationException("Exception on purpose");
            });
        }

    }
}
