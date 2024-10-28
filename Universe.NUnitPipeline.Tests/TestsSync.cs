using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Universe.NUnitPipeline;
using Universe.NUnitPipeline.Tests;

namespace Tests
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
        [TestCase("First", 7)]
        [TestCase("Next", 200)]
        public void SuccessSynchronously(string title, int milliseconds)
        {
            TestCleaner.OnDispose("Delete File '::Temporary.Temp' (from test body)", () => File.Delete("::Temporary.Temp"), TestDisposeOptions.Global);
            CpuLoad.RunSync(milliseconds);
            TestCleaner.OnDispose("Delete File TestCase.Temp (from test body)", () => File.Delete("TestCase.Temp"), TestDisposeOptions.TestCase);
        }

        [Test]
        [Category("Fail")]
        [TestCase("First", 7)]
        [TestCase("Next", 200)]
        public void FailSynchronously(string title, int milliseconds)
        {
	        CpuLoad.RunSync(milliseconds);
			Assert.Fail("Fail on purpose");
        }

        [Test]
        [Category("Fail")]
        [TestCase("First", 7)]
        [TestCase("Next", 200)]
        public void ExceptionSynchronously(string title, int milliseconds)
        {
	        CpuLoad.RunSync(milliseconds);
			throw new InvalidOperationException("Exception on purpose");
        }
    }
}
