using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                var names = new[] { "GITHUB_ACTIONS", "TF_BUILD" };
                bool isBuildServer = false;
                foreach(var name in names)
                {
                    var raw = Environment.GetEnvironmentVariable(name);
                    if (raw != null && !"False".Equals(raw, StringComparison.OrdinalIgnoreCase))
                        isBuildServer = true;
                }

                if (isBuildServer)
                {
                    var process = TryAndForget.Evaluate(() => Process.GetCurrentProcess().ProcessName);
                    if (process != null && process.IndexOf("nunit", StringComparison.OrdinalIgnoreCase) >= 0)
                        return false;
                }

                return true;
            }
        }
    }
}
