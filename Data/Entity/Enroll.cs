using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace GuildRaidBot.Data.Entity
{
    [Table("Enroll")]
    public class Enroll
    {
        [Key]
        public ulong EnrollID { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public string Server { get; set; } = string.Empty;
        public string Faction { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string SubClass { get; set; } = string.Empty;
        public string History { get; set; } = string.Empty;
        public string RegisterDatetime { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string DiscordName { get; set; } = string.Empty;
        public ulong DiscordID { get; set; }
        [ExplicitKey]
        public ulong ScheduleID { get; set; }
    }
}
