using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Universe.NUnitPipeline.Tests
{
    [TestFixture]
    public class TestShell
    {
        [Test]
        public void ShowInfo()
        {
            // dotnet test console, and VS Test Explorer: "testhost|testhost.net48"
            // ReSharper: ReSharperTestRunner
            // nunit-runner 3.18: nunit-agent
            Console.WriteLine($"PROCESS: {Process.GetCurrentProcess().ProcessName}");
        }
    }
}
