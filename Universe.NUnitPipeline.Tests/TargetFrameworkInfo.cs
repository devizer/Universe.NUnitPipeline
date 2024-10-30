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
		public static string ShortNUnitTarget => GetNUnitTargetFramework(GetFullName(typeof(ITest))) ?? "Unknown Target";
		public static string ShortPipelineTarget => GetNUnitTargetFramework(GetFullName(typeof(NUnitPipelineChain))) ?? "Unknown Target";

		public static string GetNUnitVersion()
		{
			var asm = typeof(ITest).GetAssemblyOfType();
			return asm.GetName().Version?.ToString() ?? "vUnknown";
		}

		static string GetNUnitTargetFramework(string name)
		{
			if (name == null) return null;
			var arr = name.Split(new[] { ',', '=' });
			if (arr.Length >= 3)
			{
				return arr.First().TrimStart('.') + " " + arr.Last();
			}

			return null;
		}
		public static string GetFullName(Type type)
		{
			return type.GetAssemblyOfType()
				.GetAttributesOfAssembly(typeof(TargetFrameworkAttribute))
				.OfType<TargetFrameworkAttribute>()
				.FirstOrDefault()
				?.FrameworkName;
		}
	}
}
