using System.Reflection;

namespace Universe.NUnitPipeline
{
	public class TestInformation
	{
		public string[] StructuredFullName { get; set; } // tail is optional TestName
		public string FormattedIndex { get; set; }
		public int? FixtureIndex { get; set; }
		public int? TestIndex { get; set; } // If applied to test only
		public string TestName { get; set; }
		public MethodInfo Method { get; set; }

	}
}
