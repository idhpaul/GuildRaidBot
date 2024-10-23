using GuildRaidBot.Core.Enum;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildRaidBot.Config
{
    public struct ConfigValue
    {
        public ulong GuildID { get; init; }
        public ulong RegisterChannelID { get; init; }
        public ulong ConfirmChannelID { get; init; }
        public ulong InquireCategoryID { get; init; }
        public string SqliteDbName { get; init; }
    }

    public class BotConfig
    {
        private readonly string _name = "GRB";
        private readonly string _version = "0.0.15.beta";
        private readonly string _lastUpdateDate = "24.10.23.";
        private readonly string _description = "길드 레이드 일정 등록 및 관리용 봇입니다.";

        private readonly string _sqliteDbPath = default!;
        private readonly string _sqliteConnection = default!;

        public ConfigValue ConfigValue { get; private init; }

        public BotConfig(ConfigValue configValue)
        {
            ConfigValue = configValue;

            _sqliteDbPath = $"{Path.Combine(Environment.CurrentDirectory, ConfigValue.SqliteDbName)}";
            _sqliteConnection = $"Data Source={ConfigValue.SqliteDbName}";
        }
    }
}
