using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

using Dapper;
using System.Data.SQLite;

using GuildRaidBot.Config;
using GuildRaidBot.Util;
using GuildRaidBot.Data.Entity;
using Dapper.Contrib.Extensions;
using System.Diagnostics;
using GuildRaidBot.Core.Enum;
using Discord.WebSocket;
using System.Reactive.Concurrency;

namespace GuildRaidBot.Data
{
    internal class SqliteConnector
    {
        private readonly BotConfig _config;

        public SqliteConnector(BotConfig config)
        {
            _config = config;

            Log.Debug("SQLiteConnection constructor called");
        }

        public void Initialize()
        {
            Log.Information($"Sqlite db name : {_config.ConfigValue.SqliteDbName}");
            Log.Information($"Sqlite db path : {_config.SqliteDbPath}");
            Log.Information($"Sqlite db connection string : {_config.SqliteConnection}");

            try
            {
                using SQLiteConnection sqliteConnection = new SQLiteConnection(_config.SqliteConnection);

                if (!File.Exists(_config.SqliteDbPath))
                {
                    Log.Information("Sqlite db created.");
                    SQLiteConnection.CreateFile(_config.ConfigValue.SqliteDbName);
                }

                createScheduleTable();
                createRegistrationTable();
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.GetType()}/{ex}");
                throw;
            }
            

        }

        // DB transection
        //      Insert  <-> Add
        //      Delete  <-> Remove
        //      Select  <-> Get
        private void createScheduleTable()
        {
            using SQLiteConnection sqliteConnection = new SQLiteConnection(_config.SqliteConnection);

            sqliteConnection.Open();

            string sql = @"
                 CREATE TABLE IF NOT EXISTS Schedule (
                    ScheduleID INTEGER PRIMARY KEY,
                    Title TEXT,
                    Difficult TEXT DEFAULT '',
                    Goal TEXT DEFAULT '',
                    Datetime TEXT DEFAULT '',
                    LeaderDiscordName TEXT,
                    LeaderDiscordID INTEGER
                    );
                ";

            sqliteConnection.Execute(sql);

        }

        private void createRegistrationTable()
        {
            using SQLiteConnection sqliteConnection = new SQLiteConnection(_config.SqliteConnection);

            sqliteConnection.Open();

            // 테이블 생성 쿼리
            var sql = @"
                CREATE TABLE IF NOT EXISTS Registration (
                    RegistrationID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nickname TEXT, 
                    Server TEXT DEFAULT '',
                    Faction TEXT, 
                    Class TEXT, 
                    SubClass TEXT, 
                    History TEXT DEFAULT '',
                    RegisterDatetime TEXT, 
                    State TEXT,
                    DiscordName TEXT DEFAULT '',
                    DiscordID INTEGER,
                    ScheduleID INTEGER,
                    FOREIGN KEY (ScheduleID) REFERENCES Schedule(ScheduleID) ON DELETE CASCADE
                    );
                ";

            sqliteConnection.Execute(sql);
        }

        private async Task<Registration?> selectRegistrationOrNull(ulong scheduleID, ulong userID)
        {
            Debug.Assert(scheduleID > 0);
            Debug.Assert(userID > 0);

            using SQLiteConnection sqliteConnection = new SQLiteConnection(_config.SqliteConnection);

            string sql = "SELECT * FROM Registration WHERE ScheduleID = @ScheduleID AND DiscordID = @DiscordID";

            return await sqliteConnection.QueryFirstOrDefaultAsync<Registration>(sql, new { ScheduleID = scheduleID, DiscordID = userID });

        }

        // The prefix 'Add' in the function means database 'Insert' transection
        public void AddSchedule(Schedule schedule)
        {
            Debug.Assert(schedule is not null);

            using SQLiteConnection sqliteConnection = new SQLiteConnection(_config.SqliteConnection);

            sqliteConnection.Open();

            sqliteConnection.Insert<Schedule>(schedule);
        }

        public void AddRegistration(Registration enroll)
        {
            Debug.Assert(enroll is not null);

            using SQLiteConnection sqliteConnection = new SQLiteConnection(_config.SqliteConnection);

            sqliteConnection.Open();

            sqliteConnection.Insert<Registration>(enroll);
        }

        // The prefix 'Get' in the function means database 'Select' transcetion
        public async Task<Registration?> GetRegistrationOrNull(SocketMessageComponent message)
        {
            Debug.Assert(message is not null);

            // Get Message ID
            ulong messageID = message.Message.Id;
            // Get Interaction User ID
            ulong userID = message.User.Id;

            // Get for existence
            return await this.selectRegistrationOrNull(messageID, userID);

        }

        //public List<ExerciseEntity> DbSelectExercise(ulong ChannelId)
        //{
        //    using var sqliteConnection = new SQLiteConnection(_config.botDbConnectionString);

        //    var sql = "SELECT * FROM Exercise WHERE ChannelId = @ChannelId";
        //    var exercises = sqliteConnection.Query<ExerciseEntity>(sql, new { ChannelId = ChannelId }).ToList();

        //    return exercises;
        //}

        //public void DbInsertExercise(ExerciseEntity exercise)
        //{
        //    using var sqliteConnection = new SQLiteConnection(_config.botDbConnectionString);

        //    var sql = "INSERT INTO Exercise (ExerciseTime, CaloriesBurned, OtherData, UserName, UserId, ChannelId) VALUES (@ExerciseTime, @CaloriesBurned, @OtherData, @UserName, @UserId, @ChannelId)";
        //    {
        //        var rowsAffected = sqliteConnection.Execute(sql, exercise);
        //        Console.WriteLine($"{rowsAffected} row(s) inserted.");
        //    }
        //}
        //public void DbUpdateChallenge(ChallengeEntity challenge)
        //{
        //    using var sqliteConnection = new SQLiteConnection(_config.botDbConnectionString);

        //    var sql = "UPDATE Challenge (Title, MessageId, ChannelId, LeaderName, LeaderId) VALUES (@Title, @MessageId, @ChannelId, @LeaderName, @LeaderId)";
        //    {
        //        var rowsAffected = sqliteConnection.Execute(sql, challenge);
        //        Console.WriteLine($"{rowsAffected} row(s) updated.");
        //    }
        //}

        //public void DbDeleteChallenge(ChallengeEntity challenge)
        //{
        //    using var sqliteConnection = new SQLiteConnection(_config.botDbConnectionString);

        //    var sql = "DELETE FROM Challenge WHERE MessageId = @MessageId";
        //    {
        //        var rowsAffected = sqliteConnection.Execute(sql, challenge);
        //        Console.WriteLine($"{rowsAffected} row(s) deleted.");
        //    }
        //}
    }
}
