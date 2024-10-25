using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MyTests;
using NUnit.Framework;
using Universe;
using Universe.NUnitPipeline;

/*[assembly: NUnitGlobalDispose]*/
/*[assembly: NUnitPipelineActionAttribute]*/
/*[assembly: NUnitConsoleOutputAction]*/

/*[assembly: NUnitPipeline]*/

namespace MyTests
{

    /*[NUnitGlobalDispose]*/
    [TestFixture]
    public class TestsSync
    {
        [SetUp]
        public void Setup()
        {
            TestCleaner.OnDispose("Delete File Temporary.Temp (from test's *SetUp*)", () => File.Delete("Temporary.Temp"), TestDisposeOptions.Global);
        }
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            TestCleaner.OnDispose("Delete File Temporary.Temp (from test's *OneTimeSetUp*)", () => File.Delete("Temporary.Temp"), TestDisposeOptions.Global);
            // Test --> Class, Does Not work, Only Global for onetime startup/teardown
        }
        [TearDown]
        public void TearDown()
        {
            TestCleaner.OnDispose("Delete File Temporary.Temp (from test's *TearDown*)", () => File.Delete("Temporary.Temp"), TestDisposeOptions.Global);
        }
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            TestCleaner.OnDispose("Delete File Temporary.Temp (from test's *OneTimeTearDown*)", () => File.Delete("Temporary.Temp"), TestDisposeOptions.Global);
        }


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
                TimeSpan.FromSeconds(10*60 + 123),
                TimeSpan.FromSeconds(123.4),
                TimeSpan.FromSeconds(2.345),
                TimeSpan.FromSeconds(0.123),
                TimeSpan.FromSeconds(0.0123),
            };
            foreach (var timeSpan in array)
            {
                Console.WriteLine($"TIMESPAN {timeSpan} --> \"{ElapsedFormatter.FormatElapsed(timeSpan)}\"");
            }
            TestCleaner.OnDispose("ASYNC Delete File AsyncTemporary.Temp (from test body)", () => File.WriteAllText($"AsyncTemporary {DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.Tmp", "OK"), TestDisposeOptions.TestCase | TestDisposeOptions.Async);
            TestCleaner.OnDispose("Delete File GlobalAsyncTemporary.Temp (from test body)", () => File.Delete("GlobalAsyncTemporary.Temp"), TestDisposeOptions.Global | TestDisposeOptions.Async);
        }

        [Test]
        [TestCase("First 222 milliseconds")]
        [TestCase("Next 222 milliseconds")]
        public void Test1(string title)
        {
            TestCleaner.OnDispose("Delete File '::Temporary.Temp' (from test body)", () => File.Delete("::Temporary.Temp"), TestDisposeOptions.Global);
            CpuLoad.RunSync(222);
            TestCleaner.OnDispose("Delete File TestCase.Temp (from test body)", () => File.Delete("TestCase.Temp"), TestDisposeOptions.TestCase);
        }

        [Test]
        public void SimpleFail()
        {
            Assert.Fail("Fail on purpose");
        }

        [Test]
        public void SimpleException()
        {
            throw new InvalidOperationException("Exception on purpose");
        }
    }


    /*[NUnitGlobalDispose]*/
    [TestFixture]
    public class TestsAsync
    {
        [Test]
        [TestCase("First")]
        /*[TestCase("Next")]*/
        public async Task Test1Async_111_Milliseconds(string title)
        {
            TestCleaner.OnDispose("Delete File Temporary.Temp (from test body)", () => File.Delete("Temporary.Temp"), TestDisposeOptions.Global);
            Console.WriteLine(PropertyBagVisualizer.ShowHumanString("SYNCHRONOUS"));
            await CpuLoad.RunAsync(111);
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