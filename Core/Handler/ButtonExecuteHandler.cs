using Discord;
using Discord.WebSocket;
using GuildRaidBot.Config;
using GuildRaidBot.Util;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildRaidBot.Core.Handler
{
    public class ButtonExecuteHandler
    {
        private readonly BotConfig _config;
        private readonly DiscordSocketClient _client;

        public ButtonExecuteHandler(BotConfig config, DiscordSocketClient client)
        {
            _config = config;
            _client = client;

            Log.Debug("ButtonExecuteHandler constructor called");
        }

        public void Initialize()
        {
            _client.ButtonExecuted += OnMessageReceivedAsync;
        }

        private async Task OnMessageReceivedAsync(SocketMessageComponent component)
        {
            
        }
    }
}
