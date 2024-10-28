using System;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework.Interfaces;

namespace Universe.NUnitPipeline
{
	[AttributeUsage(AttributeTargets.Parameter)]
	public class BeautyParameterAttribute : Attribute
	{
		// Nullable. If Null then Title is parameter name
		public string Title { get; set; } = null;
		public bool Visible { get; set; } = true;
	}

	public static class BeautyTestCaseNameExtension
	{
		public static string GetBeautyTestCaseName(this ITest test)
		{
			object[] testArguments = test.Arguments;
			MethodInfo methodInfo = test.Method?.MethodInfo;
			ParameterInfo[] methodParameters = methodInfo?.GetParameters();
			if (testArguments.Length == 0) return test.Name;
			if (methodParameters?.Length != testArguments.Length) return test.Name;

			var letsDebug = "ok";
			StringBuilder argumentsBuilder = new StringBuilder();
			for (int i = 0; i < testArguments.Length; i++)
			{
				var methodParameter = methodParameters[i];
				BeautyParameterAttribute beautyAttribute = methodParameter?.GetCustomAttributes(typeof(BeautyParameterAttribute), true).OfType<BeautyParameterAttribute>().FirstOrDefault();
				bool isNameVisible = beautyAttribute?.Visible ?? false;
				if (argumentsBuilder.Length > 0) argumentsBuilder.Append(",");
				var argumentValueAsString = testArguments[i] == null ? "null" : Convert.ToString(testArguments[i]);
				bool needQuotes = !(testArguments[i] != null && testArguments[i] is ValueType);
				if (needQuotes) argumentValueAsString = $"\"{argumentValueAsString}\"";
				if (isNameVisible)
				{
					string title = beautyAttribute.Title ?? methodParameter.Name;
					argumentsBuilder.Append($"{title}={argumentValueAsString}");
				}
				else
				{
					argumentsBuilder.Append(argumentValueAsString);
				}
			}

			return $"{methodInfo.Name}({argumentsBuilder})";
		}
	}

}
