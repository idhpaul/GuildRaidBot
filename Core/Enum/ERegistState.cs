using Discord;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildRaidBot.Core.Enum
{
    public enum ERegistState
    {
        [Description("신청 확인 중")]
        WAIT,
        [Description("신청 수락")]
        ACCEPT,
        [Description("신청 거절")]
        REJECT,
        [Description("취소 신청 확인 중")]
        CANCEL_WAIT,
        [Description("취소 완료")]
        CANCEL_ACCEPT,
    }
}
