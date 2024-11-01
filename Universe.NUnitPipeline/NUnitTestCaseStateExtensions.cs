extern alias nunit;
using nunit::NUnit.Framework.Interfaces;
using nunit::NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Universe.NUnitPipeline
{
	extern alias nunit;

	public static class NUnitTestCaseStateExtensions
	{
		private static readonly string PipelinePropertyName = "NUnitPipelineCollection";
        internal class MutableDictionary
        {
	        public Dictionary<string, object> Values { get; } = new Dictionary<string, object>();
        }
        
        // Mandatory: Should be locked by a Monitor with serializable access
        public static T GetPropertyOrAdd<T>(this ITest test, string propertyName, /* nullable */ Func<ITest, T> getPropertyValue)
        {
            var tempCopy = test.Properties[PipelinePropertyName];
            // version 1
            MutableDictionary mutableDictionary = null;
            foreach (var v2 in tempCopy)
            {
                if (v2 is MutableDictionary md2)
                {
	                mutableDictionary = md2;
                    break;
                }
            }

            bool isDictionaryFound = mutableDictionary != null;
            if (!isDictionaryFound)
            {
	            mutableDictionary = new MutableDictionary();
				test.Properties.Set(PipelinePropertyName, mutableDictionary);
            }

            bool isFound = mutableDictionary.Values.TryGetValue(propertyName, out var rawValue);
            if (isFound)
	            return (T)(object)rawValue;
            else if (getPropertyValue != null)
            {
	            T state = getPropertyValue(test);
	            mutableDictionary.Values[propertyName] = state;
	            return state;
            }
            else
	            return default(T);
        }

        public static T GetPropertyOrAdd<T>(this TestContext.TestAdapter test, string propertyName, /* nullable */ Func<TestContext.TestAdapter, T> getPropertyValue)
        {
	        var tempCopy = test.Properties[PipelinePropertyName];
	        // version 1
	        MutableDictionary mutableDictionary = null;
	        foreach (var v2 in tempCopy)
	        {
		        if (v2 is MutableDictionary md2)
		        {
			        mutableDictionary = md2;
			        break;
		        }
	        }

	        bool isDictionaryFound = mutableDictionary != null;
	        if (!isDictionaryFound)
	        {
		        // TODO: No Action or Exception?
		        if (getPropertyValue != null)
			        throw new InvalidOperationException($"Can't update value '{propertyName}' outside of pipeline context. Please add assembly and/or class attribute [NUnitPipelineAction]");
		        else
			        return default(T);
	        }

	        bool isFound = mutableDictionary.Values.TryGetValue(propertyName, out var rawValue);
	        if (isFound)
		        return (T)(object)rawValue;
	        else if (getPropertyValue != null)
	        {
		        T state = getPropertyValue(test);
		        mutableDictionary.Values[propertyName] = state;
		        return state;
	        }
	        else
		        return default(T);
        }


		public static void SetProperty<T>(this ITest test, string propertyName, T newValue)
        {
	        var tempCopy = test.Properties[PipelinePropertyName];
	        // version 1
	        MutableDictionary mutableDictionary = null;
	        foreach (var v2 in tempCopy)
	        {
		        if (v2 is MutableDictionary md2)
		        {
			        mutableDictionary = md2;
			        break;
		        }
	        }

	        bool isDictionaryFound = mutableDictionary != null;
	        if (!isDictionaryFound)
	        {
		        mutableDictionary = new MutableDictionary();
		        test.Properties.Set(PipelinePropertyName, mutableDictionary);
	        }

			mutableDictionary.Values[propertyName] = newValue;
        }

		public static T GetProperty<T>(this TestContext.TestAdapter test, string propertyName)
        {
	        return GetPropertyOrAdd<T>(test, propertyName, null);
        }


	}
}
