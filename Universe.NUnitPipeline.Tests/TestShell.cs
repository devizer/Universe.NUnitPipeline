using System;
using System.Diagnostics;
using NUnit.Framework;
using Universe.NUnitPipeline;

namespace Tests
{
    [NUnitPipelineAction]
    [TestFixture]
    public class TestShell
    {
        [Test]
        public void ShowProcessInfo()
        {
            // dotnet test console, and VS Test Explorer: "testhost|testhost.net48"
            // ReSharper: ReSharperTestRunner
            // nunit-runner 3.18: nunit-agent|dotnet
            Console.WriteLine($"PROCESS: {Process.GetCurrentProcess()}");

        }


    }
}
