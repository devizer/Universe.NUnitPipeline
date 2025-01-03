using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Universe.NUnitPipeline;

namespace Tests
{
    public class CpuLoad
    {

#if !NET35 && !NET40 && !NET45 && !NET20
		public static async Task RunAsync(int milliseconds)
        {
            await Task.Factory.StartNew(() =>
            {
                RunImplementation(milliseconds);
                Console.WriteLine(PropertyBagVisualizer.ShowHumanString("A-SYNCHRONOUS"));
                TestCleaner.OnDispose("Delete CLASS.TMP (from test body)", () => { }, TestDisposeOptions.Class);
                TestCleaner.OnDispose("Delete CLASS.ASYNC.TMP (from test body)", () => {}, TestDisposeOptions.AsyncClass);
            });
        }
#endif

		public static void RunSync(int milliseconds)
        {
            RunImplementation(milliseconds);
            Console.WriteLine(PropertyBagVisualizer.ShowHumanString("SYNCHRONOUS"));
        }

        private static void RunImplementation(int milliseconds)
        {
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < milliseconds)
            {
                var mem = Marshal.AllocHGlobal(6000 * 1024);
                Marshal.FreeHGlobal(mem);
            }
        }
    }
}
