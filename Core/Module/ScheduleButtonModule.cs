using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GuildRaidBot.Config;
using GuildRaidBot.Core.Enum;
using GuildRaidBot.Core.Handler;
using GuildRaidBot.Data;
using GuildRaidBot.Data.Entity;
using GuildRaidBot.Util;

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

        private string messageFormatAlreadyExist(string existInfo)
        {
            Debug.Assert(existInfo is not null);

            return "## Notice\n" +
                    "> 💡 일정 신청 정보 입니다.\n" +
                    $"{existInfo}\n" +
                    "> 🔻 **<신청 취소>**, **<직업 변경>**, **<기타 문의>**";
        }

        private string messageFormatSucsessRegister()
        {
            return "## Notice\n> 💡 일정 신청이 완료되었습니다.";
        }

        private string messageFormatNeedRegister()
        {
            return "## Notice\n> 💡 일정 신청이 필요합니다.";
        }

        private string messageFormatRequestInquire()
        {
            return "## Notice\n> 💡 문의 요청이 되었습니다.";
        }

        private MessageComponent InquireButton(ulong scheduleMessageID)
        {
            return new ComponentBuilder()
                                    .WithButton("취소, 변경, 기타 문의", $"bt_inquire:{scheduleMessageID}", ButtonStyle.Danger, new Emoji("📞"))
                                    .Build();
        }

        [ComponentInteraction("bt_regist:*")]
        public async Task Regist(EClass @class)
        {
            try
            {
                SocketMessageComponent? interaction = Context.Interaction as SocketMessageComponent;
                if (interaction is null)
                {
                    Log.Error("Interaction casting failed at Regist");
                    await RespondAsync("Interaction casting failed", ephemeral: true);
                }
                else
                {
                    Registration? registration = await _sqlite.GetRegistrationOrNull(interaction);
                    if (registration is not null)
                    {
                        string table = StringTableViewUtil.ConvertTableView(registration);

                        // registration data to 
                        await RespondAsync(messageFormatAlreadyExist($"```{table}```"), components: InquireButton(interaction.Message.Id), ephemeral: true);
                    }
                    else
                    {
                        await Context.Interaction.RespondWithModalAsync<ScheduleRegistModal>($"md_id_schedule_regist:{@class}");
                    }
                }
            }
            catch (Exception ex)
            {
                await RespondAsync($"{ex.GetType()}/{ex}", ephemeral: true);
                throw;
            }
            

        }

        [ComponentInteraction("bt_regist_status")]
        public async Task RegistStatus()
        {
            try
            {
                SocketMessageComponent? interaction = Context.Interaction as SocketMessageComponent;
                if (interaction is null)
                {
                    Log.Error("Interaction casting failed at Regist");
                    await RespondAsync("Interaction casting failed", ephemeral: true);
                }
                else
                {
                    Registration? registration = await _sqlite.GetRegistrationOrNull(interaction);
                    if (registration is not null)
                    {
                        string table = StringTableViewUtil.ConvertTableView(registration);

                        // registration data to 
                        await RespondAsync(messageFormatAlreadyExist($"```{table}```"), components: InquireButton(interaction.Message.Id), ephemeral: true);
                    }
                    else
                    {
                        await RespondAsync(messageFormatNeedRegister(), ephemeral: true);
                    }
                }

            }
            catch (Exception ex)
            {
                await RespondAsync($"{ex.GetType()}/{ex}", ephemeral: true);
                throw;
            }

            
        }

        [ComponentInteraction("bt_inquire:*")]
        public async Task Inquire(ulong scheduleMessageID)
        {
            await Context.Interaction.RespondWithModalAsync<ScheduleInquireModal>($"md_id_schedule_inquire:{scheduleMessageID}");
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
            await RespondAsync(messageFormatSucsessRegister(), ephemeral:true);

            // Input DB
            try
            {
                // Get Schedule ID from Message ID
                SocketModal interaction = (SocketModal)Context.Interaction;

                // Insert DB
                _sqlite.AddRegistration(new Data.Entity.Registration
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
                    ScheduleID = interaction.Message.Id
                });

                Log.Information($"새로운 신청이 왔습니다.{modal.NickName}({Context.User.GlobalName})/{interaction.Message.Embeds.First().Title}");
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.GetType()}/{ex}");
                throw;
            }

        }
    
        public class ScheduleInquireModal : IModal
        {
            public string Title => "✏️ 문의 양식";

            [RequiredInput(true)]
            [InputLabel("❗ 문의 내용(<신청 취소>, <직업 변경>, <기타 문의> 등)")]
            [ModalTextInput("md_lb_si_reason",
                                style:TextInputStyle.Paragraph,
                                placeholder: "문의 하실 내용을 입력해주세요.")]
            public required string Reason { get; set; }

        }

        [ModalInteraction("md_id_schedule_inquire:*")]
        public async Task ScheduleInquireModalResponse(ulong scheduleMessageID, ScheduleInquireModal modal)
        {

            await RespondAsync(messageFormatRequestInquire(), ephemeral: true);

            await Task.Delay(5000);
            await DeleteOriginalResponseAsync();


            // Check for duplicates thread channel name

            // Create thread channel

            // Tag User and Admin

            // Unregist button


        }

    }
}
