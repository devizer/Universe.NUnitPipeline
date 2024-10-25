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

            int hoursTotalInt = (int)Math.Floor(totalSeconds / 3600);
            int daysTotalInt = hoursTotalInt / 24;

            if (totalSeconds <= 0.09)
                return elapsed.TotalMilliseconds.ToString("n2") + " milliseconds";

            else if (totalSeconds <= 99.99)
                return elapsed.TotalMilliseconds.ToString("n1") + " milliseconds";

            else if (totalSeconds <= 50 * 60)
                // return elapsed.ToString("mm':'ss'.'f");
                return new DateTime(0).Add(elapsed).ToString("mm':'ss'.'f");

            else if (totalSeconds <= 20 * 3600)
                // return new DateTime(0).Add(elapsed).ToString("HH':'mm':'ss'.'f");
                return hoursTotalInt.ToString("00") + ":" + new DateTime(0).Add(elapsed).ToString("mm':'ss'.'f");
            else
            {
                // return elapsed.ToString("d'.'hh':'mm':'ss'.'f");
                return daysTotalInt.ToString("0") + "." + (hoursTotalInt % 24).ToString("00") + ":" + new DateTime(0).Add(elapsed).ToString("mm':'ss'.'f");
            }
        }

    }
}
