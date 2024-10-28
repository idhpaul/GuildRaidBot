using Discord;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildRaidBot.Util
{
    public static class DateTimeUtil
    {
        public static DateTime StringFormatToDatetime(string input, string format)
        {
            Debug.Assert(input is not null);
            Debug.Assert(format is not null);

            return DateTime.ParseExact(input, format, null);
        }
    }
}
