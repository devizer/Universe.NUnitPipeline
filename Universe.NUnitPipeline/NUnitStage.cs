using System;
using System.Reflection;

namespace Universe.NUnitPipeline
{
    public class NUnitStage
    {
        public NUnitActionAppliedTo NUnitActionAppliedTo { get; set; }
        public NUnitActionSide Side { get; set; }
        public int? FixtureIndex { get; set; }
        public int? TestIndex { get; set; } // If applied to test only
        public string FixtureFullName { get; set; }
        public string[] StructuredFullName { get; set; } // tail is optional TestName

        public Type FixtureType { get; set; }

        // If applied to test only
        public string TestName { get; set; }
        public MethodInfo Method { get; set; }

        public string FormattedIndex =>
            FixtureIndex.HasValue ? TestIndex.HasValue ? $"{FixtureIndex.Value}.{TestIndex.Value}" : $"{FixtureIndex.Value}" : "";

        public string InternalKey { get; set; }
    }
}
