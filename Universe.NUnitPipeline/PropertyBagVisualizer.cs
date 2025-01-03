extern alias nunit;
using nunit::NUnit.Framework.Interfaces;
using nunit::NUnit.Framework;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Universe.NUnitPipeline
{
    public class PropertyBagVisualizer
    {
        public static string ShowHumanString(string title)
        {
            string[] emptyStrings = new string[0];
            var temp = string.Join(", ", (TestContext.CurrentContext?.Test?.Properties?.Keys ?? emptyStrings).Select(x => $"'{x}' is {GetTypeDescription(TestContext.CurrentContext?.Test?.Properties[x])}").ToArray());
            var ret = $"{title} Properties: {temp} (TID is {Thread.CurrentThread.ManagedThreadId})";
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
                {
                    ret = $"{item.GetType()}[] Length={copy.Length}";
                    if (item is NUnitTestCaseStateExtensions.MutableDictionary md)
                    {
                        // ret = $"{(mv.Value == null ? "mutable null" : $"[{mv.Value.GetType().Name} {{{mv.Value}}}]")}";
                        var keys = md.Values.Keys.Select(k => $"\"{k}\"").ToArray();
                        ret = $"MutableDictionary {{{string.Join(", ", keys)}}}";
                    }
                }
            }
            return ret;
        }
    }
}
