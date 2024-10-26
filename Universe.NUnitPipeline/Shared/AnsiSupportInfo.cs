using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.NUnitPipeline.Shared
{
    internal class AnsiSupportInfo
    {
        public static bool IsAnsiSupported
        {
            get
            {
                var raw = Environment.GetEnvironmentVariable("TF_BUILD");
                return raw != null && !"False".Equals(raw, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
