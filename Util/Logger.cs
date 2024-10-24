using Discord.Commands;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;
using Discord.Interactions;
using Discord.WebSocket;
using System.Diagnostics;

namespace GuildRaidBot.Util
{
    public class Logger : IDisposable
    {
        public readonly Serilog.Core.Logger log = default!;

        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactionService;

        public Logger(DiscordSocketClient client, InteractionService handler)
        {
            _client = client;
            _interactionService = handler;

            _client.Log += logHandler;
            _interactionService.Log += logHandler;

            log = (Program._mode == Core.Enum.EProgramMode.Dev)
                ? new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.Console()
                    .WriteTo.File("./log/log.txt",
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true)
                    .CreateLogger()
                : new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File("./log/log.txt",
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true)
                    .CreateLogger();

            Log.Logger = log;
            Log.Information("=-=-=-=-=-=-=-=-=-");
        }

        public void Dispose()
        {
            Log.Information("Logger dispose called");

            Log.CloseAndFlush();
        }

        private Task logHandler(LogMessage message)
        {
            if (message.Exception is CommandException cmdException)
            {
                switch (message.Severity)
                {
                    case LogSeverity.Critical:
                        Log.Fatal(cmdException, $"[Command/{message.Severity}] {cmdException.Command.Aliases[0]}"
                            + $" failed to execute in {cmdException.Context.Channel}.");
                        break;
                    case LogSeverity.Error:
                        Log.Error(cmdException, $"[Command/{message.Severity}] {cmdException.Command.Aliases[0]}"
                            + $" failed to execute in {cmdException.Context.Channel}.");
                        break;
                    case LogSeverity.Warning:
                        Log.Warning(cmdException, $"[Command/{message.Severity}] {cmdException.Command.Aliases[0]}"
                            + $" failed to execute in {cmdException.Context.Channel}.");
                        break;
                    case LogSeverity.Info:
                        Log.Information(cmdException, $"[Command/{message.Severity}] {cmdException.Command.Aliases[0]}"
                            + $" failed to execute in {cmdException.Context.Channel}.");
                        break;
                    case LogSeverity.Verbose:
                        Log.Verbose(cmdException, $"[Command/{message.Severity}] {cmdException.Command.Aliases[0]}"
                            + $" failed to execute in {cmdException.Context.Channel}.");
                        break;
                    case LogSeverity.Debug:
                        Log.Debug(cmdException, $"[Command/{message.Severity}] {cmdException.Command.Aliases[0]}"
                            + $" failed to execute in {cmdException.Context.Channel}.");
                        break;
                    default:
                        Debug.Fail("unknown type");
                        break;
                }
            }
            else if(message.Exception is Exception exception)
            {
                log.Fatal(exception, $"[Command/{message.Severity}] {exception.Message}"
                            + $" failed to execute in {exception.StackTrace}.");
            }
            else
            {
                switch (message.Severity)
                {
                    case LogSeverity.Critical:
                        Log.Fatal($"General - {message}");
                        break;
                    case LogSeverity.Error:
                        Log.Error($"General - {message}");
                        break;
                    case LogSeverity.Warning:
                        Log.Warning($"General - {message}");
                        break;
                    case LogSeverity.Info:
                        Log.Information($"General - {message}");
                        break;
                    case LogSeverity.Verbose:
                        Log.Verbose($"General - {message}");
                        break;
                    case LogSeverity.Debug:
                        Log.Debug($"General - {message}");
                        break;
                    default:
                        Debug.Fail("unknown type");
                        break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
