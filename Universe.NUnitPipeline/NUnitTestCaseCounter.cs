extern alias nunit;
using System.Collections.Generic;
using nunit::NUnit.Framework.Interfaces;

namespace Universe.NUnitPipeline
{
	extern alias nunit;

	public class TestCaseIndex
    {
        public int ClassIndex { get; set; }
        
        public int TestIndex { get; set; }

        public override string ToString()
        {
            return $"{ClassIndex}.{TestIndex}";
        }
    }

    public static class NUnitTestCaseCounter
    {
        private static readonly Dictionary<string, ClassCounterState> ClassCounters = new Dictionary<string, ClassCounterState>();
        private static readonly object ClassCounterSync = new object();
        public static readonly string COUNTER_PROPERTY_NAME = "Test Case Index";

        class ClassCounterState
        {
            public int ClassIndex;
            public int TestCount;
        }

        public static TestCaseIndex GetTestCaseIndex(this ITest test)
        {
            // Assembly is incorrect argument
            bool isConcreteTest = test.Method != null;
            string classFullName = isConcreteTest ? test.Method?.TypeInfo?.FullName : test.Fixture?.GetType().FullName;
            lock (ClassCounterSync)
            {
                return test.GetPropertyOrAdd(COUNTER_PROPERTY_NAME, x => GetNextTestIndex(classFullName, isConcreteTest));
            }
        }

        static TestCaseIndex GetNextTestIndex(string classFullname, bool isConcreteTest)
        {
            int classIndex, testIndex;
            if (ClassCounters.TryGetValue(classFullname, out var classCounterState))
            {
                classIndex = classCounterState.ClassIndex;
                if (isConcreteTest) classCounterState.TestCount++;
                testIndex = classCounterState.TestCount;
            }
            else
            {
                classIndex = ClassCounters.Count + 1;
                testIndex = isConcreteTest ? 1 : 0;
                ClassCounters[classFullname] = new ClassCounterState() { TestCount = testIndex, ClassIndex = classIndex };
            }

            return new TestCaseIndex() { ClassIndex = classIndex, TestIndex = testIndex };
        }
    }
}
