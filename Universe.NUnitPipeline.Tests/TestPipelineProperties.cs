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
			Assert.IsNull(missing42, "At the beginning the property 42 is missing");

			var added42 = TestContext.CurrentContext.Test.GetPropertyOrAdd<string>("42", test => "42");
			Assert.AreEqual("42", added42, "After first addition of property '42' the value is expected");

			var again42 = TestContext.CurrentContext.Test.GetPropertyOrAdd<string>("42", null);
			Assert.AreEqual("42", again42, "After adding value the getter should return 42");

			var last42 = TestContext.CurrentContext.Test.GetPropertyOrAdd<string>("42", test => "42++");
			Assert.AreEqual("42", last42, "Property can not be changed after it is added");

			Assert.That(last42, Is.EqualTo("42"), "Property can not be changed after it is added");

		}
	}
}
