using Discord;
using Discord.Interactions;
using GuildRaidBot.Config;
using GuildRaidBot.Core.Attribute;
using GuildRaidBot.Core.Enum;
using GuildRaidBot.Core.Handler;
using GuildRaidBot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GuildRaidBot.Core.Module.ScheduleCreateModule;

namespace GuildRaidBot.Core.Module
{
    public class TestModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly BotConfig _config;
        private readonly InteractionHandler _handler;
        private readonly SqliteConnector _sqlite;

        TestModule(BotConfig config, InteractionHandler handler, SqliteConnector sqlite)
        {
            _config = config;
            _handler = handler;
            _sqlite = sqlite;

            Log.Debug("TestModule constructor called");
        }

        [SlashCommand("테스트", "테스트용")]
        [RequireCommandRole(ERole.Admin)]
        public async Task TestCommand()
        {
            try
            {
                string fooDate = "2024-088-10 / 18:00";
                string dateFormat = "yyyy-MM-dd / HH:mm";

                DateTime convertDatetime = DateTime.ParseExact(fooDate, dateFormat, null);
                Console.WriteLine(convertDatetime);
            }
            catch (FormatException ex)
            {
                await RespondAsync($"{ex.Message} / 날짜 형식을 맞춰주세요");
                throw;
            }
            catch (Exception)
            {
                await RespondAsync("An unexpected error occurred. Please try again.");
                throw;
            }
            



        }
    }
}
