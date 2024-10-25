using System;
using System.Collections.Generic;
using System.Text;

namespace Universe.NUnitPipeline
{
    public class ElapsedFormatter
    {
        public static string FormatElapsed(TimeSpan elapsed)
        {
            var totalSeconds = elapsed.TotalSeconds;

            if (totalSeconds <= 0.09)
                return elapsed.TotalMilliseconds.ToString("n2") + " milliseconds";

            else if (totalSeconds <= 99.99)
                return elapsed.TotalMilliseconds.ToString("n1") + " milliseconds";

            else if (totalSeconds <= 50 * 60)
                return elapsed.ToString("mm':'ss'.'f");

            else if (totalSeconds <= 20 * 3600)
                return elapsed.ToString("hh':'mm':'ss'.'f");

            else
                return elapsed.ToString("d'.'hh':'mm':'ss'.'f");
        }

    }
}
