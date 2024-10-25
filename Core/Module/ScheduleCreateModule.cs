using Discord;
using Discord.Interactions;
using GuildRaidBot.Config;
using GuildRaidBot.Core.Enum;
using GuildRaidBot.Core.Handler;
using GuildRaidBot.Util;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Threading.Channels;
using Microsoft.VisualBasic;
using GuildRaidBot.Data;
using GuildRaidBot.Core.Attribute;

namespace GuildRaidBot.Core.Module
{
    public class ScheduleCreateModule : InteractionModuleBase<SocketInteractionContext>
    {

        private readonly BotConfig _config;
        private readonly InteractionHandler _handler;
        private readonly SqliteConnector _sqlite;

        ScheduleCreateModule(BotConfig config, InteractionHandler handler, SqliteConnector sqlite)
        {
            _config = config;
            _handler = handler;
            _sqlite = sqlite;

            Log.Debug("ScheduleCreateModule constructor called");
        }

        [SlashCommand("일정등록", "공대 일정을 등록할 수 있습니다.")]
        [RequireCommandRole(ERole.Admin)]
        public async Task ScheduleCreateCommand()
        {
            await Context.Interaction.RespondWithModalAsync<ScheduleCreateModal>("md_id_schedule_create");
        }

        public class ScheduleCreateModal : IModal
        {
            public string Title => "📌 일정 등록";

            [RequiredInput(true)]
            [InputLabel("공지 본문")]
            [ModalTextInput("md_lb_sc_context",
                                style: TextInputStyle.Paragraph,
                                initValue: "### [신규 레이드 일정] @everyone \n" + 
                                            "> 🛡️  :  **전클 구인**\n" +
                                            "> ⚔️  :  **전클 구인**\n" +
                                            "> 💚  :  **전클 구인**\n" +
                                            "* ✅ 구인 클래스를 꼭 확인해 주세요.\n" +
                                            "* ✅ 레이드 시작 15분 전에 미리 접속해 주세요.\n")]
            public required string Context { get; set; }

            [RequiredInput(true)]
            [InputLabel("일정 제목")]
            [ModalTextInput("md_lb_sc_schedule_title",
                                initValue: "네룹아르 궁전")]
            public required string ScheduleTitle { get; set; }

            [RequiredInput(true)]
            [InputLabel("일정 난이도 (ex :🟧 신화)")]
            [ModalTextInput("md_lb_sc_difficult",
                                initValue: "🟩 일반 / 🟪 영웅 / 🟧 신화")]
            public required string Difficult { get; set; }

            [RequiredInput(true)]
            [InputLabel("일정 목표 및 탐수(ex : ☠️ 올킬 / ⌛ 2.5탐)")]
            [ModalTextInput("md_lb_sc_goal",
                                initValue: "☠️ 올킬 / ⌛ 0탐")]
            public required string Goal { get; set; }

            [RequiredInput(true)]
            [InputLabel("날짜 (2025-01-31 / 18:00) (이 양식대로 반드시 입력해주세요.)")]
            [ModalTextInput("md_lb_sc_datetime",
                                initValue: "2024-00-00 / 00:00")]
            public required string Datetime { get; set; }

        }

        [ModalInteraction("md_id_schedule_create")]
        public async Task ScheduleCreateModalResponse(ScheduleCreateModal modal)
        {
            // Send Schedule message
            Embed scheduleEmbed = new EmbedBuilder()
                            .WithTitle(modal.ScheduleTitle)
                            .WithFields(new EmbedFieldBuilder().WithName("\u200B").WithValue($"> {modal.Difficult}").WithIsInline(false))
                            .WithFields(new EmbedFieldBuilder().WithName("\u200B").WithValue($"> {modal.Goal}").WithIsInline(false))
                            .WithFields(new EmbedFieldBuilder().WithName("\u200B").WithValue($"> {modal.Datetime}").WithIsInline(false))
                            .WithFields(new EmbedFieldBuilder().WithName("😎 *공장*").WithValue($"{Context.User.Mention}").WithIsInline(true))
                            .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())
                            .WithColor(Discord.Color.Green)
                            .Build();

            await RespondAsync(modal.Context,embed:scheduleEmbed);

            // Get Schdule message ID
            IUserMessage sentMessage = await GetOriginalResponseAsync();

            // Add Button before message
            var buttons = new ComponentBuilder()
                            .WithButton("탱커 신청", $"bt_regist:{EClass.Tank}", ButtonStyle.Primary, new Emoji(EnumUtil.GetDescription(EClass.Tank)))
                            .WithButton("근딜/원딜 신청", $"bt_regist:{EClass.Deal}", ButtonStyle.Danger, new Emoji(EnumUtil.GetDescription(EClass.Deal)))
                            .WithButton("힐러 신청", $"bt_regist:{EClass.Heal}", ButtonStyle.Success, new Emoji(EnumUtil.GetDescription(EClass.Heal)))
                            .WithButton("신청 현황 및 문의(메시지 최하단 확인)", $"bt_regist_status", ButtonStyle.Secondary,row:1)
                            .Build();


            await sentMessage.ModifyAsync(message => message.Components = buttons);

            // Input DB
            try
            {
                _sqlite.DbInsertSchedule(new Data.Entity.Schedule
                {
                    Title = modal.ScheduleTitle,
                    Difficult = modal.Difficult,
                    Goal = modal.Goal,
                    Datetime = modal.Datetime,
                    LeaderDiscordName = Context.User.GlobalName,
                    LeaderDiscordID = Context.User.Id,
                    DiscordMessageID = sentMessage.Id
                });

                Log.Information($"신규 일정이 등록되었습니다.{modal.ScheduleTitle}/{modal.Difficult}/{modal.Datetime}/{Context.User.GlobalName}");
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.GetType()}/{ex}");
                throw;
            }

        }
    }
}
