using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Universe
{
    public static class NUnitTestCaseStateExtensions
    {
        private static readonly object _TestCaseStateSync = new object();

        class MutableValue
        {
            public object Value { get; set; }
        }
        
        // Mandatory: Should be locked by a Monitor with serializable access
        public static T GetPropertyOrAdd<T>(this ITest test, string propertyName, /* nullable */ Func<ITest, T> getPropertyValue)
        {
            // Console.WriteLine($"[DEBUG] SET {propertyName} for '{test.Name}'");
            T state = default(T);
            bool isFound = false;
            var rawArray = ((IEnumerable)test.Properties[propertyName]).Cast<IEnumerable<object>>().ToArray();
            var mutableValue = rawArray.OfType<MutableValue>().FirstOrDefault();
            isFound = mutableValue != null;
            if (isFound) state = (T)(object)mutableValue.Value;

            if (isFound) 
                return state;
            else if (getPropertyValue != null)
            {
                state = getPropertyValue(test);
                test.Properties.Set(propertyName, new MutableValue() { Value = state });
                return state;
            }
            else
                return default(T);
        }

        public static void SetProperty<T>(this ITest test, string propertyName, T newValue)
        {
            T state = default(T);
            bool isFound = false;
            var rawArray = ((IEnumerable)test.Properties[propertyName]).Cast<IEnumerable<object>>().ToArray();
            var mutableValue = rawArray.OfType<MutableValue>().FirstOrDefault();
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
            var rawArray = ((IEnumerable)test.Properties[propertyName]).Cast<IEnumerable<object>>().ToArray();
            var mutableValue = rawArray.OfType<MutableValue>().FirstOrDefault();
            isFound = mutableValue != null;
            if (isFound) state = (T)(object)mutableValue.Value;

            if (isFound)
                return state;

            return default(T);
        }
    }
}