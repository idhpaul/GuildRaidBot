using Discord;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildRaidBot.Core.Enum
{

    public enum EClass
    {
        [Description("🛡️")]
        Tank,
        [Description("⚔️")]
        Deal,
        [Description("🤍")]
        Heal
    }
}
