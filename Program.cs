using System.Xml.Linq;
using System.Diagnostics;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using GuildRaidBot.Config;
using GuildRaidBot.Core.Handler;
using GuildRaidBot.Core.Enum;
using GuildRaidBot.Data;
using GuildRaidBot.Util;
using GuildRaidBot.Core.Module;


namespace GuildRaidBot;

internal class Program
{
    private static IServiceProvider _services = default!;
    public readonly static EProgramMode Mode = EProgramMode.Dev;

    public static async Task Main(string[] args)
    {
        using (IHost host = new HostBuilder().Build())
        {
            var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

            lifetime.ApplicationStarted.Register(() =>
            {
                Log.Information($"Started / Mode : {Mode}");
            });
            lifetime.ApplicationStopping.Register(() =>
            {
                Log.Information("Stopping firing");
                Log.Information("Stopping end");
            });
            lifetime.ApplicationStopped.Register(() =>
            {
                Log.Information("Stopped firing");
                Log.Information("Stopped end");
            });

            host.Start();

            try
            {
                IConfigurationRoot secretKey = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

                string discordToken = getTokenFromSecretJson(secretKey);
                ConfigValue configValue = getConfigValueFromSecretJson(secretKey);

                // Setup DI container.
                _services = configureServices(configValue);

                using var client = _services.GetRequiredService<DiscordSocketClient>();
                using var interactionService = _services.GetRequiredService<InteractionService>();
                using var logger = _services.GetRequiredService<Logger>();

                await _services.GetRequiredService<InteractionHandler>().Initialize();
                _services.GetRequiredService<MessageHandler>().Initialize();
                _services.GetRequiredService<ButtonExecuteHandler>().Initialize();
                _services.GetRequiredService<SqliteConnector>().Initialize();

                // Login and connect.
                await client.LoginAsync(TokenType.Bot, discordToken);
                await client.StartAsync();

                // Wait.
                host.WaitForShutdown();

            }
            catch (Exception)
            {
                Console.WriteLine("Check Exception");
            }
        }
    }

    private static string getTokenFromSecretJson(IConfigurationRoot secretKey)
    {
        string? discordToken = (Mode == EProgramMode.Live) ? secretKey["DiscordToken"] : secretKey["Dev:DiscordToken"];
        Debug.Assert(discordToken is not null);
        if (discordToken is null)
        {
            throw new Exception($"Check DiscordToken value at secret.json file  / " +
                                $"DiscordToken : {discordToken ?? "null"}");
        }

        return discordToken;
    }

    private static ConfigValue getConfigValueFromSecretJson(IConfigurationRoot secretKey)
    {

        // Get Config value at secrets.json
        ulong guildID = Convert.ToUInt64((Mode == EProgramMode.Live) ? secretKey["Config:GuildID"] : secretKey["Dev:Config:GuildID"]);
        ulong registerChannelID = Convert.ToUInt64((Mode == EProgramMode.Live) ? secretKey["Config:RegisterChannelID"] : secretKey["Dev:Config:RegisterChannelID"]);
        ulong confirmChannelID = Convert.ToUInt64((Mode == EProgramMode.Live) ? secretKey["Config:ConfirmChannelID"] : secretKey["Dev:Config:ConfirmChannelID"]);
        ulong inquireCategoryID = Convert.ToUInt64((Mode == EProgramMode.Live) ? secretKey["Config:InquireCategoryID"] : secretKey["Dev:Config:InquireCategoryID"]);
        string? sqliteDbName = (Mode == EProgramMode.Live) ? secretKey["Config:SqliteDbName"] : secretKey["Dev:Config:SqliteDbName"];

        Debug.Assert(guildID != 0 );
        Debug.Assert(registerChannelID != 0 );
        Debug.Assert(confirmChannelID != 0 );
        Debug.Assert(inquireCategoryID != 0 );
        Debug.Assert(sqliteDbName is not null );

        // check value
        if (guildID == 0 && 
            registerChannelID == 0 && 
            confirmChannelID == 0 && 
            inquireCategoryID == 0 && 
            sqliteDbName is null)
        {
            throw new Exception($"Check Config value at secret.json file  / " +
                $"GuildID : {guildID}, " +
                $"RegisterChannelID : {guildID}, " +
                $"ConfirmChannelID : {guildID}, " +
                $"InquireCategoryID : {guildID}, " +
                $"SqliteDbName : {sqliteDbName ?? "null"}"
                );
        }

        return new ConfigValue
        {
            GuildID = guildID,
            RegisterChannelID = registerChannelID,
            ConfirmChannelID = confirmChannelID,
            InquireCategoryID = inquireCategoryID,
            SqliteDbName = sqliteDbName
        };
        
    }

    private static ServiceProvider configureServices(ConfigValue configValue)
    {
        var serviceCollection = new ServiceCollection()
        .AddSingleton<BotConfig>(new BotConfig(configValue))
        .AddSingleton<DiscordSocketClient>(provider =>
        {
            var discordSocketConfig = new DiscordSocketConfig
            {
                LogLevel = (Mode == EProgramMode.Live) ? LogSeverity.Info : LogSeverity.Verbose,
                MessageCacheSize = 100,
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers | GatewayIntents.MessageContent,
                AlwaysDownloadUsers = true,
            };

            return new DiscordSocketClient(discordSocketConfig);
        })
        .AddSingleton<InteractionService>(provider =>
        {
            var interactionConfig = new InteractionServiceConfig
            {
                LogLevel = (Mode == EProgramMode.Live) ? LogSeverity.Info : LogSeverity.Verbose,
            };

            return new InteractionService(provider.GetRequiredService<DiscordSocketClient>().Rest, interactionConfig);
        })
        .AddSingleton<InteractionHandler>()
        .AddSingleton<MessageHandler>()
        .AddSingleton<ButtonExecuteHandler>()
        .AddSingleton<SqliteConnector>()
        .AddSingleton<Logger>();

        return serviceCollection.BuildServiceProvider();
    }
}

