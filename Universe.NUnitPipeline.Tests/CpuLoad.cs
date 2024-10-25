using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Universe.NUnitPipeline.Tests
{
    public class CpuLoad
    {
        public static async Task RunAsync(int milliseconds)
        {
            await Task.Run(() =>
            {
                RunImplementation(milliseconds);
                Console.WriteLine(PropertyBagVisualizer.ShowHumanString("A-SYNCHRONOUS"));
            });
        }

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