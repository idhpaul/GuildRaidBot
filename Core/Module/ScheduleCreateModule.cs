using Discord.Interactions;
using GuildRaidBot.Config;
using GuildRaidBot.Core.Handler;
using GuildRaidBot.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace GuildRaidBot.Core.Module
{
    public class ScheduleCreateModule
    {

        private readonly BotConfig _config;
        private readonly InteractionHandler _handler;
        private readonly Logger _log;

        ScheduleCreateModule(BotConfig config, InteractionHandler handler, Logger log)
        {
            _config = config;
            _handler = handler;
            _log = log;

            _log.Log.Information("ChallengeCreateModule constructor called");
        }

        [SlashCommand("일정등록", "[공대장 전용] 공대 일정을 등록할 수 있습니다.")]
        [RequireCommandRole(Role.Leader)]
        public async Task RegistChallenge()
        {
            await Context.Interaction.RespondWithModalAsync<ChallengeCreateModalContext>("md_id_createChallenge");
        }

    }
}
