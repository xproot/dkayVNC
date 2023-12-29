using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace dkayVNC
{
    internal class Bot
    {
        public static DiscordClient client;
        public static Config clientConfig;

        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            var logFactory = new LoggerFactory().AddSerilog();

            //Get config
            if (File.Exists("config.json"))
            {
                clientConfig = JsonSerializer.Deserialize<Config>(File.ReadAllText("config.json"));
            } else
            {
                Log.Fatal("No config found!!!!!!!!!!!!!!");
                File.WriteAllText("config.json", JsonSerializer.Serialize(new Config(), new JsonSerializerOptions { WriteIndented = true}));
                Log.Information("A default config has been made in the current path.");
                Environment.Exit(1);
            }

            client = new DiscordClient(new DiscordConfiguration()
            {
                Token = clientConfig.BotToken,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                LoggerFactory = logFactory
            });
            var commands = client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = clientConfig.BotPrefixes,
                EnableMentionPrefix = clientConfig.BotPingPrefix
            });

            commands.RegisterCommands(Assembly.GetExecutingAssembly());

            client.Ready += OnReadyHandler;
            client.GuildDownloadCompleted += OnGuildDownloadHandler;

            await client.ConnectAsync();
            await Task.Delay(-1);
        }

        static private Task OnReadyHandler(DiscordClient client, ReadyEventArgs e)
        {
            Log.Information($"Connected as: {client.CurrentUser.Username}#{client.CurrentUser.Discriminator} ({client.CurrentUser.Id})");
            Log.Information("Discord Client is ready");
            return Task.CompletedTask;
        }

        static private Task OnGuildDownloadHandler(DiscordClient client, GuildDownloadCompletedEventArgs e)
        {
            Log.Information($"Discord Client is in {client.Guilds.Count} guilds");
            foreach (DiscordGuild guild in e.Guilds.Values)
            {
                Log.Information($"- Guild: \"{guild.Name}\" ({guild.Id})");
            }
            return Task.CompletedTask;
        }
    }

    public class Config
    {
        public string BotToken { get; set; } = "BOT_TOKEN_HERE";
        public UInt64[] BotOwners { get; set; } = { 366188463198044162, 831176009507536937 };
        public string[] BotPrefixes { get; set; } = { "dkay!" , "dkayvnc." };
        public bool BotPingPrefix { get; set; } = true;
        public UInt64 DefaultBindToChannel { get; set; } = 942766066550067230;
        public string DefaultConnectToHost { get; set; } = "";
        public UInt16 DefaultConnectToPort { get; set; } = 5900;
        public string DefaultConnectToPassword { get; set; } = "Password goes here...";
    }
}
