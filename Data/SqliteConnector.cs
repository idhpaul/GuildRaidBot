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
            Log.Debug($"Sqlite db name : {_config.ConfigValue.SqliteDbName}");
            Log.Debug($"Sqlite db path : {_config.SqliteDbPath}");
            Log.Debug($"Sqlite db connection string : {_config.SqliteConnection}");

            try
            {
                using SQLiteConnection sqliteConnection = new SQLiteConnection(_config.SqliteConnection);

                if (!File.Exists(_config.SqliteDbPath))
                {
                    Log.Debug("Sqlite db 파일 생성");
                    SQLiteConnection.CreateFile(_config.ConfigValue.SqliteDbName);
                }

                CreateScheduleTable();
                CreateEnrollTable();
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.GetType()}/{ex}");
                throw;
            }
            

        }

        private void CreateScheduleTable()
        {
            using SQLiteConnection sqliteConnection = new SQLiteConnection(_config.SqliteConnection);

            sqliteConnection.Open();

            string scheduleTable = @"
                 CREATE TABLE IF NOT EXISTS Schedule (
                    ScheduleID INTEGER PRIMARY KEY,
                    Title TEXT,
                    Difficult TEXT DEFAULT '',
                    Goal TEXT DEFAULT '',
                    Datetime TEXT DEFAULT '',
                    LeaderDiscordName TEXT,
                    LeaderDiscordID INTEGER,
                    DiscordMessageID INTEGER
                    );
                ";

            sqliteConnection.Execute(scheduleTable);

        }

        private void CreateEnrollTable()
        {
            using SQLiteConnection sqliteConnection = new SQLiteConnection(_config.SqliteConnection);

            sqliteConnection.Open();

            // 테이블 생성 쿼리
            var enrollTable = @"
                CREATE TABLE IF NOT EXISTS Enroll (
                    EnrollID INTEGER PRIMARY KEY AUTOINCREMENT,
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

            sqliteConnection.Execute(enrollTable);
        }

        //public async Task<ChallengeEntity> DbSelectChallenge(ulong messageId)
        //{
        //    using var sqliteConnection = new SQLiteConnection(_config.botDbConnectionString);

        //    var sql = "SELECT * FROM Challenge WHERE MessageId = @MessageId";

        //    var challenge = await sqliteConnection.QuerySingleOrDefaultAsync<ChallengeEntity>(sql, new { MessageId = messageId });
        //    return challenge;

        //}

        public void DbInsertSchedule(Schedule schedule)
        {
            using SQLiteConnection sqliteConnection = new SQLiteConnection(_config.SqliteConnection);

            sqliteConnection.Open();

            sqliteConnection.Insert<Schedule>(schedule);
        }

        public void DbInsertEnroll(Enroll enroll)
        {
            using SQLiteConnection sqliteConnection = new SQLiteConnection(_config.SqliteConnection);

            sqliteConnection.Open();

            sqliteConnection.Insert<Enroll>(enroll);
        }

        public async Task<ulong> DbGetScheduleID(ulong messageID)
        {
            using SQLiteConnection sqliteConnection = new SQLiteConnection(_config.SqliteConnection);

            string query = "SELECT * FROM Schedule WHERE DiscordMessageID = @DiscordMessageID";
            Schedule schedule = await sqliteConnection.QueryFirstOrDefaultAsync<Schedule>(query, new { DiscordMessageID = messageID });
            if (schedule is null)
            {
                throw new Exception("Invalid Query");
            }
            return schedule.ScheduleID;
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
