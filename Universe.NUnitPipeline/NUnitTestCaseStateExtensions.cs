using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Universe.NUnitPipeline
{
    public static class NUnitTestCaseStateExtensions
    {
        private static readonly object _TestCaseStateSync = new object();

        internal class MutableValue
        {
            public object Value { get; set; }
        }
        
        // Mandatory: Should be locked by a Monitor with serializable access
        public static T GetPropertyOrAdd<T>(this ITest test, string propertyName, /* nullable */ Func<ITest, T> getPropertyValue)
        {
            // Console.WriteLine($"[DEBUG] SET {propertyName} for '{test.Name}'");
            T state = default(T);
            bool isFound = false;
            var tempCopy = test.Properties[propertyName];
            // version 1
            MutableValue mutableValue = null;
            foreach (var v2 in tempCopy)
            {
                if (v2 is MutableValue mv2)
                {
                    mutableValue = mv2;
                    break;
                }
            }
            // version 2
            // var rawArray = (test.Properties[propertyName]).Cast<IEnumerable<object>>().ToArray();
            // MutableValue mutableValue = rawArray.OfType<MutableValue>().FirstOrDefault();
            isFound = mutableValue != null;
            if (isFound) state = (T)(object)mutableValue.Value;

            if (isFound) 
                return state;
            else if (getPropertyValue != null)
            {
                state = getPropertyValue(test);
                test.Properties.Add(propertyName, new MutableValue() { Value = state });
                return state;
            }
            else
                return default(T);
        }

        public static void SetProperty<T>(this ITest test, string propertyName, T newValue)
        {
            T state = default(T);
            bool isFound = false;
            var tempCopy = test.Properties[propertyName];
            // version 1
            MutableValue mutableValue = null;
            foreach (var v2 in tempCopy)
            {
                if (v2 is MutableValue mv2)
                {
                    mutableValue = mv2;
                    break;
                }
            }
            // version 2
            // var rawArray = (test.Properties[propertyName]).Cast<IEnumerable<object>>().ToArray();
            // MutableValue mutableValue = rawArray.OfType<MutableValue>().FirstOrDefault();
            isFound = mutableValue != null;
            if (isFound) state = (T)(object)mutableValue.Value;

            if (isFound)
                mutableValue.Value = newValue;
            else
                test.Properties.Set(propertyName, new MutableValue() { Value = state });
        }

        public static T GetProperty<T>(this TestContext.TestAdapter test, string propertyName)
        {
            T state = default(T);
            bool isFound = false;
            var tempCopy = test.Properties[propertyName];
            // version 1
            MutableValue mutableValue = null;
            foreach (var v2 in tempCopy)
            {
                if (v2 is MutableValue mv2)
                {
                    mutableValue = mv2;
                    break;
                }
            }
            // version 2
            // var rawArray = (test.Properties[propertyName]).Cast<IEnumerable<object>>().ToArray();
            // MutableValue mutableValue = rawArray.OfType<MutableValue>().FirstOrDefault();
            isFound = mutableValue != null;
            if (isFound) state = (T)(object)mutableValue.Value;

            if (isFound)
                return state;

            return default(T);
        }
    }
}
