using System;
using System.Collections.Generic;
using System.Linq;

namespace Universe.NUnitPipeline.ConsoleTreeTable
{
	public static class DictionaryExtensions
	{
		public static V GetOrAdd<K, V>(this IDictionary<K, V> dictionary, K key, Func<K, V> getNewValue)
		{
			if (dictionary.TryGetValue(key, out var ret))
				return ret;

			ret = getNewValue(key);
			dictionary.Add(key, ret);
			return ret;
		}

		public static Dictionary<TKey, TElement> ToSafeDictionary<TSource, TKey, TElement>(
			this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector,
			Func<TSource, TElement> elementSelector)
		{
			Dictionary<TKey, TElement> ret = new Dictionary<TKey, TElement>();
			// var copy = source.ToArray();
			var index = 0;
			foreach (var src in source)
			{
				var key = keySelector(src);
				var value = elementSelector(src);
				if (key == null) throw new ArgumentException($"Element's #{index} key is null", nameof(source));
				// TODO: Multiple Same Names are allowed.
				// if (ret.ContainsKey(key)) continue; else ret.Add(key, value);
				ret[key] = value;
			}

			return ret;
		}


	}
}
