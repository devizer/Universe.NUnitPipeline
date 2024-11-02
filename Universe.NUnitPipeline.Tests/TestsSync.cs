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
    public class TestsSynchronously
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
        [TestCase("First", 7)]
        [TestCase("Next", 200)]
        public void SynchronousSuccess(string title, [BeautyParameter] int milliseconds)
        {
            TestCleaner.OnDispose("Delete File '::Temporary.Temp' (from test body)", () => File.Delete("::Temporary.Temp"), TestDisposeOptions.Global);
            CpuLoad.RunSync(milliseconds);
            TestCleaner.OnDispose("Delete File TestCase.Temp (from test body)", () => File.Delete("TestCase.Temp"), TestDisposeOptions.TestCase);
        }

        [Test]
        [Category("Fail")]
        [TestCase("First", 7)]
        [TestCase("Next", 200)]
        public void SynchronousFail(string title, [BeautyParameter] int milliseconds)
        {
	        CpuLoad.RunSync(milliseconds);
			Assert.Fail("Fail on purpose");
        }

        [Test]
        [Category("Fail")]
        [TestCase("First", 7)]
        [TestCase("Next", 200)]
        public void SynchronousException(string title, [BeautyParameter] int milliseconds)
        {
	        CpuLoad.RunSync(milliseconds);
			throw new InvalidOperationException("Exception on purpose");
        }
    }
}
