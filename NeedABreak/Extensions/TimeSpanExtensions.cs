using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeedABreak.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string ToHumanFriendlyString(this TimeSpan value)
        {
            string res;

            if (value < TimeSpan.FromMinutes(1))
            {
                res = string.Format(Properties.Resources.n_minutes, value);
            }
            else if (value < TimeSpan.FromMinutes(2))
            {
                res = Properties.Resources.one_minute;
            }
            else if (value < TimeSpan.FromHours(1))
            {
                res = string.Format(Properties.Resources.n_minutes, value);
            }
            else if (value.Hours == 1 && value.Minutes == 0)
            {
                res = string.Format(Properties.Resources.one_hour, value);
            }
            else if (value.Hours == 1)
            {
                res = string.Format(Properties.Resources.one_hour_n_minutes, value);
            }
            else if (value.Minutes == 0)
            {
                res = string.Format(Properties.Resources.n_hours, value);
            }
            else
            {
                res = string.Format(Properties.Resources.n_hours_n_minutes, value);
            }

            return res;
        }
    }
}
