using MyTests;

namespace Universe.NUnitPipeline.Tests;

[NUnitPipelineAction]
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
    [Explicit, Category("Fail")]
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
    [Explicit, Category("Fail")]
    public async Task AsyncException()
    {
        await Task.Run(function: async () =>
        {
            await CpuLoad.RunAsync(42);
            throw new InvalidOperationException("Exception on purpose");
        });
    }

}