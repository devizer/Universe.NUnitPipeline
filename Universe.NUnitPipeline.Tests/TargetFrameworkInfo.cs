extern alias nunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using nunit::NUnit.Compatibility;
using nunit::NUnit.Framework.Interfaces;
using Universe.NUnitPipeline;

namespace Tests
{
	internal class TargetFrameworkInfo
	{
		public static string ShortNUnitTarget => GetShortTargetFramework(typeof(ITest));
		
		public static string PipelineTarget => GetShortTargetFramework(typeof(NUnitPipelineChain));
		public static string PipelineVersion => GetVersion(typeof(NUnitPipelineChain));

		public static string NUnitVersion => GetVersion(typeof(ITest));

		private static string GetShortTargetFramework(Type type)
		{
			return GetShortTargetByFull(GetFullTargetNameName(type)) ?? "Unknown Target";
		}


		private static string GetVersion(Type type)
		{
			var asm = type.GetAssemblyOfType();
			return asm.GetName().Version?.ToString() ?? "vUnknown";
		}

		static string GetShortTargetByFull(string name)
		{
			if (name == null) return null;
			var arr = name.Split(new[] { ',', '=' });
			if (arr.Length >= 3)
			{
				return arr.First().TrimStart('.') + " " + arr.Last();
			}

			return null;
		}
		public static string GetFullTargetNameName(Type type)
		{
			return type.GetAssemblyOfType()
				.GetAttributesOfAssembly(typeof(TargetFrameworkAttribute))
				.OfType<TargetFrameworkAttribute>()
				.FirstOrDefault()
				?.FrameworkName;
		}
	}
}
