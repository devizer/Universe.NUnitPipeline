using System;

namespace Universe.NUnitPipeline.Shared
{
    internal class AnsiSupportInfo
    {
        public static bool IsAnsiSupported
        {
            get
            {
                if ("True".Equals(Environment.GetEnvironmentVariable("ENABLE_ANSI_COLORS"))) return true;
                return false;

                var names = new[] { "GITHUB_ACTIONS", "TF_BUILD" };
                bool isBuildServer = false;
                foreach(var name in names)
                {
                    var raw = Environment.GetEnvironmentVariable(name);
                    if (raw != null && !"False".Equals(raw, StringComparison.OrdinalIgnoreCase))
                        isBuildServer = true;
                }

                /*
                if (isBuildServer)
                {
                    var process = TryAndForget.Evaluate(() => Process.GetCurrentProcess().ProcessName);
                    bool isNunit = process != null && process.IndexOf("nunit", StringComparison.OrdinalIgnoreCase) >= 0;
                    bool isDotnet = process != null && process.Equals("dotnet", StringComparison.OrdinalIgnoreCase);
                    if (isNunit /*|| isDotnet#1#) return false;
                }

                return isBuildServer;
            */
            }
        }
    }
}
