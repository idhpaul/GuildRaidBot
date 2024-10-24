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
                    schedule_ID INTEGER PRIMARY KEY,
                    title TEXT,
                    difficult TEXT DEFAULT '',
                    goal TEXT DEFAULT '',
                    datetime TEXT DEFAULT '',
                    leader_discord_name TEXT,
                    leader_discord_ID INTEGER
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
                    enroll_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    nickname TEXT, 
                    server TEXT DEFAULT '',
                    faction TEXT, 
                    class TEXT, 
                    sub_class TEXT, 
                    history TEXT DEFAULT '',
                    register_date TEXT, 
                    state TEXT,
                    discord_ID INTEGER,
                    discord_name TEXT DEFAULT '',
                    schedule_ID INTEGER,
                    FOREIGN KEY (schedule_ID) REFERENCES schedule(schedule_ID) ON DELETE CASCADE
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

        //public void DbInsertChallenge(ChallengeEntity challenge)
        //{
        //    using var sqliteConnection = new SQLiteConnection(_config.botDbConnectionString);

        //    var sql = "INSERT INTO Challenge (Title, MessageId, ChannelId, LeaderName, LeaderId) VALUES (@Title, @MessageId, @ChannelId, @LeaderName, @LeaderId)";
        //    {
        //        var rowsAffected = sqliteConnection.Execute(sql, challenge);
        //        Console.WriteLine($"{rowsAffected} row(s) inserted.");
        //    }
        //}

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
