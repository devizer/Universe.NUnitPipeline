using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        public static void WaitAll()
        {
            List<WaitHandle> copy;
            lock (Sync)
            {
                copy = new List<WaitHandle>(Waiters);
                Waiters.Clear();
            }

            foreach (var waitHandle in copy)
            {
                waitHandle.WaitOne();
            }
        }
    }
}
