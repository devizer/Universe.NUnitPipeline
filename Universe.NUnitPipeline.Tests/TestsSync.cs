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
    public class TestsSync
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            TestCleaner.OnDispose("Delete GLOBAL.Temp (from test's *OneTimeSetUp*)", () => { }, TestDisposeOptions.Global);
			// Test ➛ Class for onetimesetup/onetimeteardown
			TestCleaner.OnDispose("Delete TEST➛CLASS.Temp (from test's *OneTimeSetUp*)", () => { }, TestDisposeOptions.TestCase);
			Console.WriteLine("WHATS UP. I'm OneTimeSetUp that runs before any test");
        }

        [SetUp]
        public void Setup()
        {
            TestCleaner.OnDispose("Delete CLASS.Temp (from test's *SetUp*)", () => File.Delete("Temporary.Temp"), TestDisposeOptions.Class);
        }

        [TearDown]
        public void TearDown()
        {
	        TestCleaner.OnDispose("Delete File GLOBAL.Temp (from test's *TearDown*)", () => { }, TestDisposeOptions.Global);
	        TestCleaner.OnDispose("Delete File CLASS.Temp (from test's *TearDown*)", () => { }, TestDisposeOptions.Class);
			TestCleaner.OnDispose("Delete File TEST.Temp (from test's *TearDown*)", () => { }, TestDisposeOptions.TestCase);
        }

		[OneTimeTearDown]
        public void OneTimeTearDown()
        {
	        TestCleaner.OnDispose("Delete File GLOBAL.Temp (from test's *OneTimeTearDown*)", () => { }, TestDisposeOptions.Global);
			TestCleaner.OnDispose("Delete File CLASS.Temp (from test's *OneTimeTearDown*)", () => { }, TestDisposeOptions.Class);
			TestCleaner.OnDispose("Delete File TEST➛CLASS.Temp (from test's *OneTimeTearDown*)", () => { }, TestDisposeOptions.TestCase);
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

            TestCleaner.OnDispose("ASYNC Delete File AsyncTemporary.Temp (from test body)", () => File.WriteAllText($"AsyncTemporary {DateTime.Now:yyyy-MM-dd HH-mm-ss}.Tmp", "OK"), TestDisposeOptions.AsyncTestCase);
            TestCleaner.OnDispose("Delete File GlobalAsyncTemporary.Temp (from test body)", () => File.Delete("GlobalAsyncTemporary.Temp"), TestDisposeOptions.AsyncGlobal);
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
        [Explicit, Category("Fail")]
        public void SimpleFail()
        {
            Assert.Fail("Fail on purpose");
        }

        [Test]
        [Explicit, Category("Fail")]
        public void SimpleException()
        {
            throw new InvalidOperationException("Exception on purpose");
        }
    }
}
