extern alias nunit;
using System;
using nunit::NUnit.Framework;
using Universe.NUnitPipeline;

namespace Tests
{
	[NUnitPipelineAction]
	[TestFixture]
	public class TestPipelineProperties
	{
		[Test]
		public void Test42()
		{
			Console.WriteLine(PropertyBagVisualizer.ShowHumanString("SYNC"));
			// TODO: Async test

			var missing42 = TestContext.CurrentContext.Test.GetPropertyOrAdd<string>("42", null);
			if (missing42 != null) Assert.Fail("At the beginning the property 42 is missing");

			var added42 = TestContext.CurrentContext.Test.GetPropertyOrAdd<string>("42", test => "42");
			if (added42 != "42") Assert.Fail("After first addition of property '42' the value is expected");

			var again42 = TestContext.CurrentContext.Test.GetPropertyOrAdd<string>("42", null);
			if (again42 != "42") Assert.Fail("After adding value the getter should return 42");

			var last42 = TestContext.CurrentContext.Test.GetPropertyOrAdd<string>("42", test => "42++");
			if (last42 != "42") Assert.Fail("Property can not be changed after it is added");
		}
	}
}
