using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.NUnitPipeline
{
	[AttributeUsage(AttributeTargets.Parameter)]
	public class BeautyParameterAttribute : Attribute
	{
		// Nullable. If Null then Title is parameter name
		public string Title { get; set; }
		public bool Visible { get; set; } = true;
	}
}
