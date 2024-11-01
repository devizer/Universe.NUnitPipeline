using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Universe.NUnitPipeline
{
	public static class NUnitPipelineConfiguration
	{
		// TODO: Concurrent Dictionary
		private static Dictionary<object, Lazy<object>> Services = new Dictionary<object, Lazy<object>>();

		static NUnitPipelineConfiguration()
		{
			Register(() => new NUnitPipelineChain());
			Register(() => new NUnitReportConfiguration());
		}

		public static T GetService<T>()
		{
			if (!Services.TryGetValue(typeof(T), out var raw))
				throw new ArgumentException($"The requested services <{typeof(T).Name}> is not registered");

			return (T)raw.Value;
		}

		public static void Register<T>(Func<T> lazy)
		{
			if (Services.TryGetValue(typeof(T), out var raw) && raw.IsValueCreated)
				throw new ArgumentException($"The requested services <{typeof(T).Name}> is already registered");

			Services[typeof(T)] = new Lazy<object>(() => lazy(), LazyThreadSafetyMode.ExecutionAndPublication);
		}
	}
}
