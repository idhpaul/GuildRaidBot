using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using GuildRaidBot.Core.Enum;
using GuildRaidBot.Util;

namespace GuildRaidBot.Core.Attribute
{
    public class RequireRoleAttribute(ERole role) : Discord.Commands.PreconditionAttribute
    {
        private readonly ERole _role = role;

        public override Task<Discord.Commands.PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            // Check if this user is a Guild User, which is the only context where roles exist
            if (context.User is SocketGuildUser gUser)
            {
                // Get Role Description
                string roleDescription = EnumUtil.GetDescription(_role);

                if (gUser.Roles.Any(r => r.Name == roleDescription))
                    return Task.FromResult(Discord.Commands.PreconditionResult.FromSuccess());
                else
                    return Task.FromResult(Discord.Commands.PreconditionResult.FromError($"`{roleDescription}` 전용 명령어 입니다."));
            }
            else
                return Task.FromResult(Discord.Commands.PreconditionResult.FromError("길드 전용 명령어 입니다."));
        }
    }
}
