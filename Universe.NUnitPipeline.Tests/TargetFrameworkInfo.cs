extern alias nunit;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using nunit::NUnit.Compatibility;
using nunit::NUnit.Framework.Interfaces;

namespace Tests
{
	internal class TargetFrameworkInfo
	{
		public static string ShortName => GetShortName(GetFullName()) ?? "Unknown Target";

		static string GetShortName(string name)
		{
			if (name == null) return null;
			var arr = name.Split(new[] { ',', '=' });
			if (arr.Length >= 3)
			{
				return arr.First().TrimStart('.') + " " + arr.Last();
			}

			return null;
		}
		public static string GetFullName()
		{
			return typeof(ITest).GetAssemblyOfType()
				.GetCustomAttributes(typeof(TargetFrameworkAttribute), false)
				.OfType<TargetFrameworkAttribute>()
				.FirstOrDefault()
				?.FrameworkName;
		}
	}
}
