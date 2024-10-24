using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Threading;
using GuildRaidBot.Config;
using GuildRaidBot.Util;
using Serilog;


namespace GuildRaidBot.Core.Handler
{
    public class MessageHandler
    {
        private readonly BotConfig _config;
        private readonly DiscordSocketClient _client;

        public MessageHandler(BotConfig config, DiscordSocketClient client)
        {
            _config = config;
            _client = client;

            Log.Debug("MessageHandler constructor called");
        }

        public void Initialize()
        {
            _client.MessageReceived += OnMessageReceivedAsync;
        }


        private async Task OnMessageReceivedAsync(SocketMessage arg)
        {
            // 봇이 보낸 메시지인 경우 무시
            if (arg.Author.IsBot) return;
            if (arg is not IUserMessage message) return;

            switch (message.Type)
            {
                case MessageType.Default:
                case MessageType.Reply:
                    break;
                default:
                    // 기타 메시지 유형 처리
                    break;
            }
        }
    }
}
