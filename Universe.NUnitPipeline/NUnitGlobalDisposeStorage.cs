using System;
using System.Collections.Generic;
// using Action=[]

namespace Universe.NUnitPipeline
{
	internal class NUnitGlobalDisposeStorage
    {
        private static Dictionary<string, List<System.Action>> OnDisposeList = new Dictionary<string, List<System.Action>>();
        private static readonly object OnDisposeListSync = new object();

        internal static void AddDisposeAction(string collectionKey, System.Action action)
        {
            lock (OnDisposeListSync)
            {
                if (!OnDisposeList.TryGetValue(collectionKey, out var list))
                {
                    list = OnDisposeList[collectionKey] = new List<System.Action>();
                }

                list.Add(action);
            }
        }

        public static List<System.Action> FetchDisposeActions(string collectionKey)
        {
            lock (OnDisposeListSync)
            {
                if (OnDisposeList.TryGetValue(collectionKey, out var list))
                {
                    OnDisposeList[collectionKey] = new List<System.Action>();
                    return new List<System.Action>(list);
                }
            }

            return null;
        }

    }
}
