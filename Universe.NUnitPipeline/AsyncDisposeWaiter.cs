using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Universe.NUnitPipeline;

namespace Universe {
    internal class AsyncDisposeWaiter
    {
        public class WaiterCount
        {
            public int Total { get; set; }
            public int Completed { get; set; }
            public int Incomplete { get; set; }
        }

        static readonly List<WaitHandle> Waiters = new List<WaitHandle>();
        static readonly object Sync = new object();

        public static void AddWaiter(WaitHandle waiter)
        {
            lock(Sync) Waiters.Add(waiter);
        }

        public static WaiterCount GetCount()
        {
            List<WaitHandle> copy;
            lock (Sync) copy = new List<WaitHandle>(Waiters);
            int incomplete = copy.Count(x => !x.WaitOne(0));
            int count = copy.Count;
            return new WaiterCount()
            {
                Completed = count - incomplete,
                Incomplete = incomplete,
                Total = count
            };
        }

        public static int WaitAll()
        {
            List<WaitHandle> copy;
            lock (Sync)
            {
                copy = new List<WaitHandle>(Waiters);
                Waiters.Clear();
            }

            // TODO: Move to configuration
            int timeoutMilliseconds = 5*60*1000;
			Stopwatch startAt = Stopwatch.StartNew();
			int countWaiting;
			do
			{
				countWaiting = copy.Count(x => !x.WaitOne(0));
				if (countWaiting == 0) break;
				Thread.Sleep(42);

			} while (startAt.ElapsedMilliseconds < timeoutMilliseconds);

			if (countWaiting > 0)
				PipelineLog.LogTrace($"[DEBUG] Warning! Warning! Warning! {nameof(AsyncDisposeWaiter)}.{nameof(WaitAll)} Timeout ({timeoutMilliseconds / 1000}). {countWaiting} are still incomplete");

			return countWaiting;
        }
    }
}
