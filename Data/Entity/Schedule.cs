using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace GuildRaidBot.Data.Entity
{
    [Table("Schedule")]
    public class Schedule
    {
        [Write(true)]
        [Key]
        public ulong ScheduleID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Difficult { get; set; } = string.Empty;
        public string Goal { get; set; } = string.Empty;
        public string Datetime { get; set; } = string.Empty;
        public string LeaderDiscordName { get; set; } = string.Empty;
        public ulong LeaderDiscordID { get; set; }
        public ulong DiscordMessageID { get; set; }
    }
}
