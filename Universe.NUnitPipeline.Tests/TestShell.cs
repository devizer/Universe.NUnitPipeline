extern alias nunit;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using nunit::NUnit.Framework;
using Universe.NUnitPipeline;

namespace Tests
{
	[NUnitPipelineAction]
	[TestFixture]
	public class TestProperties
	{
		[Test]
		public void Test42()
		{
			var missing42 = TestContext.CurrentContext.Test.GetPropertyOrAdd<string>("42", null);
			Assert.IsNull(missing42, "At the beginning the property 42 is missing");

			var added42 = TestContext.CurrentContext.Test.GetPropertyOrAdd<string>("42", test => "42");
			Assert.AreEqual("42", added42, "After first add the value of 42 is expected 42");

			var again42 = TestContext.CurrentContext.Test.GetPropertyOrAdd<string>("42", null);
			Assert.AreEqual("42", again42, "After adding value the getter should return 42");

			var last42 = TestContext.CurrentContext.Test.GetPropertyOrAdd<string>("42", test => "42++");
			Assert.AreEqual("42", last42, "Property can not be changed after add");


			Console.WriteLine(PropertyBagVisualizer.ShowHumanString("42"));
		}
	}

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
				Assembly typeAssembly = type.GetAssemblyOfType();
				string name = typeAssembly?.Location != null ? Path.GetFileName(typeAssembly?.Location) : "";
				TargetFrameworkAttribute tfa = typeAssembly.GetAttributesOfAssembly(typeof(TargetFrameworkAttribute)).OfType<TargetFrameworkAttribute>().FirstOrDefault();
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
					Assembly typeAssembly = type.GetAssemblyOfType();
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
					TargetFrameworkAttribute tfa = typeAssembly.GetAttributesOfAssembly(typeof(TargetFrameworkAttribute)).OfType<TargetFrameworkAttribute>().FirstOrDefault();
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
