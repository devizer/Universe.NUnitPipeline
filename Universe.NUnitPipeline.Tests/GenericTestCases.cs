using System;
using NUnit.Framework;
using Universe.NUnitPipeline;

namespace Tests
{
	[NUnitPipelineAction]
	[TestFixture]
	public class GenericTestCases
	{
		[TestCase(42)]
		[TestCase("Hello")]
		[TestCase(double.Epsilon)]
		public void GenericTest<T>(T instance)
		{
			Console.WriteLine(instance);
		}

		[TestCase(42)]
		[TestCase("Hello")]
		[TestCase(double.Epsilon)]
		public void BeautyGenericTest<T>([BeautyParameter] T instance)
		{
			Console.WriteLine(instance);
		}
	}
}
