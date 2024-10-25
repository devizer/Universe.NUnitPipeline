using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace Universe.NUnitPipeline
{
    public class PropertyBagVisualizer
    {
        public static string ShowHumanString(string title)
        {
            string[] emptyStrings = new string[0];
            var temp = string.Join(", ", (TestContext.CurrentContext?.Test?.Properties?.Keys ?? emptyStrings).Select(x => $"'{x}' is {GetTypeDescription(TestContext.CurrentContext?.Test?.Properties[x])}").ToArray());
            var ret = $"{title } Properties [{temp}] (Thread ID is {Thread.CurrentThread.ManagedThreadId})";
            Debug.WriteLine(ret);
            return ret;
        }

        static string GetTypeDescription(object x)
        {
            var ret = x == null ? "null" : x.GetType().ToString();
            if (x is IEnumerable enumerable)
            {
                var copy = enumerable.Cast<object>().ToArray();
                ret = $"Empty Enumerable Length={copy.Length}";
                var item = copy.FirstOrDefault();
                if (item != null)
                    ret = $"{item.GetType()}[] Length={copy.Length}";
            }
            return ret;
        }
    }
}
