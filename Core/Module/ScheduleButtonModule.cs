using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;
using GuildRaidBot.Config;
using GuildRaidBot.Core.Enum;
using GuildRaidBot.Core.Handler;
using GuildRaidBot.Data;

namespace GuildRaidBot.Core.Module
{
    public class ScheduleButtonModule : InteractionModuleBase<SocketInteractionContext>
    {

        private readonly BotConfig _config;
        private readonly InteractionHandler _handler;
        private readonly SqliteConnector _sqlite;

        ScheduleButtonModule(BotConfig config, InteractionHandler handler, SqliteConnector sqlite)
        {
            _config = config;
            _handler = handler;
            _sqlite = sqlite;

            Log.Debug("ScheduleButtonModule constructor called");
        }


        [ComponentInteraction("bt_regist:*")]
        public async Task Regist(EClass @class)
        {
            await Context.Interaction.RespondWithModalAsync<ScheduleRegistModal>($"md_id_schedule_regist:{@class}");
        }

        [ComponentInteraction("bt_regist_status")]
        public async Task RegistStatus(ulong messageID)
        {
            await Context.Interaction.RespondWithModalAsync<ScheduleRegistModal>("md_id_schedule_regist");
        }

        public class ScheduleRegistModal : IModal
        {
            public string Title => "✏️ 신청 양식";

            [RequiredInput(true)]
            [InputLabel("❗ 닉네임")]
            [ModalTextInput("md_lb_sr_nickname",
                                placeholder: "닉네임을 입력해주세요.")]
            public required string NickName { get; set; }

            [RequiredInput(true)]
            [InputLabel("❗ 서버 (아즈샤라, 하이잘, 헬스크림, 줄진 등")]
            [ModalTextInput("md_lb_sr_server",
                                placeholder: "서버를 입력해주세요.")]
            public required string Server { get; set; }

            [RequiredInput(true)]
            [InputLabel("❗ 진영 (호드 얼라)")]
            [ModalTextInput("md_lb_sr_faction",
                                initValue: "호드 얼라")]
            public required string Faction { get; set; }

            [RequiredInput(true)]
            [InputLabel("❗ 클래스(ex 양조, 악탱, 화법, 신사, 힐스왑 등)")]
            [ModalTextInput("md_lb_sr_class",
                                placeholder: "클래스를 입력해주세요.")]
            public required string Class { get; set; }

            [RequiredInput(true)]
            [InputLabel("❗ 로그 및 경험 점수(공장이 수월하게 확인하기 위함이니 제대로 적어주세요.)")]
            [ModalTextInput("md_lb_sr_history",
                                placeholder: "로그 및 경험 점수를 적어주세요.")]
            public required string History { get; set; }

        }

        [ModalInteraction("md_id_schedule_regist:*")]
        public async Task ScheduleRegistModalResponse(EClass @class, ScheduleRegistModal modal)
        {
            Log.Debug(@class.ToString());
            await RespondAsync("신청 되었습니다.",ephemeral:true);


            //// Send Regist message to RegistListChannel
            //Embed scheduleEmbed = new EmbedBuilder()
            //                .WithTitle(modal.ScheduleTitle)
            //                .WithFields(new EmbedFieldBuilder().WithName("\u200B").WithValue($"> {modal.Difficult}").WithIsInline(false))
            //                .WithFields(new EmbedFieldBuilder().WithName("\u200B").WithValue($"> {modal.Goal}").WithIsInline(false))
            //                .WithFields(new EmbedFieldBuilder().WithName("\u200B").WithValue($"> {modal.Datetime}").WithIsInline(false))
            //                .WithFields(new EmbedFieldBuilder().WithName("😎 *공장*").WithValue($"{Context.User.Mention}").WithIsInline(true))
            //                .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())
            //                .WithColor(Discord.Color.Green)
            //                .Build();

            //await RespondAsync(modal.Context, embed: scheduleEmbed);

            //// Get Schdule message ID
            //IUserMessage sentMessage = await GetOriginalResponseAsync();

            //// Add Button before message
            //var buttons = new ComponentBuilder()
            //                .WithButton("탱커 신청", $"bt_regist_tank:{sentMessage.Id}", ButtonStyle.Primary, new Emoji("🛡️"))
            //                .WithButton("근딜/원딜 신청", $"bt_regist_deal:{sentMessage.Id}", ButtonStyle.Danger, new Emoji("⚔️"))
            //                .WithButton("힐러 신청", $"bt_regist_heal:{sentMessage.Id}", ButtonStyle.Success, new Emoji("🤍"))
            //                .WithButton("신청 현황 및 문의(메시지 최하단 확인)", $"bt_regist_status:{sentMessage.Id}", ButtonStyle.Secondary, row: 1)
            //                .Build();


            //await sentMessage.ModifyAsync(message => message.Components = buttons);

            // Input DB
            try
            {
                // Get Schedule ID from Message ID
                SocketModal interaction = (SocketModal)Context.Interaction;

                ulong scheduleID = await _sqlite.DbGetScheduleID(interaction.Message.Id);
                
                // Insert DB
                _sqlite.DbInsertEnroll(new Data.Entity.Enroll
                {
                    Nickname = modal.NickName,
                    Server = modal.Server,
                    Faction = modal.Faction,
                    Class = modal.Class,
                    SubClass = @class.ToString(),
                    History = modal.History,
                    RegisterDatetime = DateTime.Now.ToString(),
                    State = ERegistState.WAIT.ToString(),
                    DiscordName = Context.User.GlobalName,
                    DiscordID = Context.User.Id,
                    ScheduleID = scheduleID
                });

                Log.Information($"새로운 신청이 왔습니다.{modal.NickName}({Context.User.GlobalName})/{interaction.Message.Embeds.First().Title}");
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.GetType()}/{ex}");
                throw;
            }

        }
    }
}
