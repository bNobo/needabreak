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
            return value.ToString();
        }
    }
}
