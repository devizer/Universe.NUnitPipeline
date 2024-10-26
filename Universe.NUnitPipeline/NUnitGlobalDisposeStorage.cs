using System;
using System.Collections.Generic;

namespace Universe.NUnitPipeline
{
    internal class NUnitGlobalDisposeStorage
    {
        private static Dictionary<string, List<Action>> OnDisposeList = new Dictionary<string, List<Action>>();
        private static readonly object OnDisposeListSync = new object();

        internal static void AddDisposeAction(string collectionKey, Action action)
        {
            lock (OnDisposeListSync)
            {
                if (!OnDisposeList.TryGetValue(collectionKey, out var list))
                {
                    list = OnDisposeList[collectionKey] = new List<Action>();
                }

                list.Add(action);
            }
        }

        public static List<Action> FetchDisposeActions(string collectionKey)
        {
            lock (OnDisposeListSync)
            {
                if (OnDisposeList.TryGetValue(collectionKey, out var list))
                {
                    OnDisposeList[collectionKey] = new List<Action>();
                    return new List<Action>(list);
                }
            }

            return null;
        }

    }
}
