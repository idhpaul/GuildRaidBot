using ConsoleTables;
using GuildRaidBot.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildRaidBot.Util
{
    public static class StringTableViewUtil
    {
        public static string ConvertTableView(Registration registration)
        {
            string table = new ConsoleTable("닉네임", "직업", "상태")
                .AddRow(registration.Nickname, registration.Class, registration.State)
                .ToStringAlternative();

            return table;
        }
    }
}
