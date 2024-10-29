using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Versioning;

namespace Tests
{
	public static class TypeExtensions {
		
		public static Assembly GetAssemblyOfType(this Type type)
		{
#if NETCOREAPP1_1 || NETCOREAPP1_0
			return type.GetTypeInfo().Assembly;
#else
            return type.Assembly;
#endif
		}

		public static IEnumerable GetAttributesOfAssembly(this Assembly assembly, Type attributeType)
		{
#if NETCOREAPP1_1 || NETCOREAPP1_0
			return attributeType == null ? assembly.GetCustomAttributes() : assembly.GetCustomAttributes(attributeType);
#else
            return attributeType == null ? assembly.GetCustomAttributes(false)  : assembly.GetCustomAttributes(attributeType, false);
#endif
		}

	}
}
