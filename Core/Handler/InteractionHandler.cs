﻿using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GuildRaidBot.Util;
using Serilog;

namespace GuildRaidBot.Core.Handler
{
    public class InteractionHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _handler;
        private readonly IServiceProvider _services;

        public InteractionHandler(DiscordSocketClient client, InteractionService handler, IServiceProvider services)
        {
            _client = client;
            _handler = handler;
            _services = services;

            Log.Debug("InteractionHandler constructor called");
        }

        public async Task Initialize()
        {
            // Process when the client is ready, so we can register our commands.
            _client.Ready += ReadyAsync;

            // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
            await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            // Process the InteractionCreated payloads to execute Interactions commands
            _client.InteractionCreated += HandleInteraction;

            // Also process the result of the command execution.
            _handler.InteractionExecuted += HandleInteractionExecute;
        }

        private async Task ReadyAsync()
        {
            //await CommandUtils.DeleteAllGlobalCommands(_client);

            // Register the commands globally.
            // alternatively you can use _handler.RegisterCommandsGloballyAsync() to register commands to a specific guild.
            await _handler.RegisterCommandsGloballyAsync();
        }

        private async Task HandleInteraction(SocketInteraction interaction)
        {
            try
            {
                // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
                var context = new SocketInteractionContext(_client, interaction);

                // Execute the incoming command.
                var result = await _handler.ExecuteCommandAsync(context, _services);

                // Due to async nature of InteractionFramework, the result here may always be success.
                // That's why we also need to handle the InteractionExecuted event.
                if (!result.IsSuccess)
                    switch (result.Error)
                    {
                        case InteractionCommandError.UnmetPrecondition:
                            // implement
                            break;
                        default:
                            break;
                    }
            }
            catch
            {
                // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if (interaction.Type is InteractionType.ApplicationCommand)
                    await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }

        private Task HandleInteractionExecute(ICommandInfo commandInfo, IInteractionContext context, IResult result)
        {
            if (!result.IsSuccess)
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        context.Interaction.RespondAsync($"{result.ErrorReason}", ephemeral: true);
                        break;
                    default:
                        break;
                }

            return Task.CompletedTask;
        }
    }
}
