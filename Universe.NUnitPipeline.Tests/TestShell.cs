using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
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

	    [Test]
	    public void ShowAssemblyTargets()
	    {
		    Console.WriteLine($"PROCESS: '{Process.GetCurrentProcess()?.ProcessName}'");
			var types = new[] { typeof(TestShell), typeof(NUnitPipelineActionAttribute) };
			foreach (var type in types)
			{
				Assembly typeAssembly = type.Assembly;
				string name = typeAssembly?.Location != null ? Path.GetFileName(typeAssembly?.Location) : "";
				TargetFrameworkAttribute tfa = typeAssembly.GetCustomAttributes(typeof(TargetFrameworkAttribute), false).OfType<TargetFrameworkAttribute>().FirstOrDefault();
				Console.WriteLine($"Assembly '{name}': \"{tfa?.FrameworkName}\"  \"{tfa?.FrameworkDisplayName}\"");
			}
	    }

        [Test] 
        public void ShowAssemblyAttributes()
		{
			var types = new[] { typeof(TestShell), typeof(NUnitPipelineActionAttribute) };
			foreach (var type in types)
			{
				try
				{
					Console.WriteLine($"type is {type.FullName}");
					Assembly typeAssembly = type.Assembly;
					Console.WriteLine($"ok: typeAssembly, {typeAssembly}");
					string l = typeAssembly?.Location;
					Console.WriteLine("ok: typeAssembly.Location");
					string a = typeAssembly?.ImageRuntimeVersion;
					Console.WriteLine("ok: typeAssembly.ImageRuntimeVersion");
					var assemblyAttributes = typeAssembly.GetCustomAttributes().ToArray();
					var aaNames = assemblyAttributes.Select(x => x.GetType().ToString()).ToArray();
					Console.WriteLine($"Assembly Attributes: [{string.Join(", ", aaNames)}]");
					foreach (var aa in assemblyAttributes)
					{
						Console.WriteLine($"{Environment.NewLine}Attribute '{aa.GetType()}' for {Path.GetFileName(l)}");
						foreach (var propertyInfo in aa.GetType().GetProperties())
						{
							var value = propertyInfo.GetValue(aa);
							Console.WriteLine($"   {propertyInfo.Name}: '{value}'");
						}
					}
					Console.WriteLine();
					TargetFrameworkAttribute tfa = typeAssembly.GetCustomAttributes(typeof(TargetFrameworkAttribute), false).OfType<TargetFrameworkAttribute>().FirstOrDefault();
					Console.WriteLine("ok: TargetFrameworkAttribute tfa ");

					Console.WriteLine($"Assembly '{l}': {a}, {tfa?.FrameworkName}; {tfa?.FrameworkDisplayName}");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"FAIL{Environment.NewLine}{ex}");
					// Debugger.Break();
					throw;
				}

			}
		}




	}
}
