using System;
using NUnit.Framework;
using Universe.NUnitPipeline;

namespace Tests
{
	[NUnitPipelineAction]
	[TestFixture]
	public class GenericTestCases
	{
		const decimal x = 1.23456m;
		const decimal y = 1.23456m;

		[TestCase(42)]
		[TestCase("Hello")]
		[TestCase(double.Epsilon)]
		[TestCase(double.Epsilon)] 
		public void GenericTest<T>(T instance)
		{
			Console.WriteLine(instance);
			decimal? x = (decimal?)1.23456m;
		}

		[TestCase(42)]
		[TestCase(42)]
		[TestCase("Hello")]
		[TestCase(double.Epsilon)]
		public void BeautyGenericTest<T>([BeautyParameter] T instance)
		{
			Console.WriteLine(instance);
		}
	}
}
