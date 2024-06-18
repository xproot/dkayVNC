using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemoteViewing.Vnc;
using Serilog;
using dkayVNC.Utils;

namespace dkayVNC
{
    internal class Program
    {
        public class BotConfig
        {
            public string[] Prefixes { get; set; } = { "d!", "dkay!" };
            public string Token { get; set; } = "BOT_TOKEN_HERE";
            public ulong[] OwnerId { get; set; } = { 366188463198044162, 831176009507536937 };
            public ulong DefaultChannelId { get; set; } = 0;
            public bool EnforceDefaultChannel { get; set; } = true;
            public string DefaultVNCServerHostname { get; set; } = "";
            public ushort DefaultVNCServerPort { get; set; } = 5900;
            public string DefaultVNCServerPassword { get; set; } = "";
            public ushort GraphicalError { get; set; } = 1;
        }

        public static string LastControl = "no one";
        public static string LastControlType = "none";
        public static ulong LastControlId = 0;
        public static Int64 LastControlTimestamp = 0;
        public static DiscordChannel CurrentBoundChannel;
        public static DiscordClient Client;
        public static CommandsNextExtension Commands;
        public static BotConfig Config = new BotConfig();
        public static VncClient RfbClient = new VncClient();
        public static string CurrentHostname = "N/A";
        public static ushort CurrentPort = 0;
        public static int CurrentX = 0;
        public static int CurrentY = 0;
        public static bool MeantToDisconnect = false;

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            var logFactory = new LoggerFactory()
                .AddSerilog();

            //Get config
            if (File.Exists("config.json"))
            {
                try { Config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText("config.json")); }
                catch { Log.Fatal("config.json is unreadable!"); return; }
            }
            else
            {
                Log.Fatal("config.json does not exist, thus:");
                File.WriteAllText("config.json", JsonConvert.SerializeObject(Config, Formatting.Indented));
                Log.Information("A default config has been made in the current path.");
                Environment.Exit(1);
            }

            Log.Logger.Information("dkayVNC");

            RfbClient.Connected += Vnc_Connection;
            RfbClient.ConnectionFailed += Vnc_ConnectionFail;
            RfbClient.Closed += Vnc_Close;
            RfbClient.RemoteClipboardChanged += Vnc_RemoteClipboardChange;

            MainAsync().GetAwaiter().GetResult();
        }

        public static async Task MainAsync()
        {
            Client = new DiscordClient(new DiscordConfiguration()
            {
                Token = Config.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                LoggerFactory = new LoggerFactory().AddSerilog()

            });

            Client.Ready += Client_Ready;
            Client.GuildDownloadCompleted += Client_GuildDownloadComplete;

            Commands = Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                EnableDefaultHelp = false,
                StringPrefixes = Config.Prefixes,
                EnableMentionPrefix = true
            });
            Commands.CommandErrored += Command_Errored;
            Commands.RegisterCommands(Assembly.GetExecutingAssembly());

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        static private Task Client_Ready(DiscordClient client, ReadyEventArgs e)
        {
            PermissionData.Init();
            Log.Information($"Connected as: {client.CurrentUser.Username}#{client.CurrentUser.Discriminator} ({client.CurrentUser.Id})");
            Log.Information("Discord Client is ALMOST READY (Please wait for Guild Download)");
            return Task.CompletedTask;
        }

        static private Task Client_GuildDownloadComplete(DiscordClient client, GuildDownloadCompletedEventArgs e)
        {
            Log.Information($"Discord Client is in {client.Guilds.Count} guilds");
            foreach (DiscordGuild guild in e.Guilds.Values)
            {
                Log.Information($"- Guild: \"{guild.Name}\" ({guild.Id})");
            }

            if (Config.DefaultChannelId > 0)
            {
                try
                {
                    CurrentBoundChannel = client.GetChannelAsync(Config.DefaultChannelId).GetAwaiter().GetResult();
                    Log.Information($"Binding to channel set in bot config: {CurrentBoundChannel.Name} ({CurrentBoundChannel.Id})");

                    if (Config.EnforceDefaultChannel)
                    {
                        LogAndBound($"ℹ #{CurrentBoundChannel.Name} is required{Environment.NewLine}dkayVNC has been configured to only accept VNC commands on {CurrentBoundChannel.Name}.");
                            
                    }
                    if (!String.IsNullOrWhiteSpace(Config.DefaultVNCServerHostname))
                    {
                        LogAndBound($"Due to the bot's configuration, {CurrentBoundChannel.Name} has been bound and a connection attempt to {Config.DefaultVNCServerHostname}:{Config.DefaultVNCServerPort} is being made.");
                        VncClientConnectOptions _connsettings = new VncClientConnectOptions { ShareDesktop = true };
                        _connsettings.Password = Config.DefaultVNCServerPassword.ToCharArray();

                        Log.Logger.Information($"Connecting to {Config.DefaultVNCServerHostname}:{Config.DefaultVNCServerPort}");
                        Program.CurrentHostname = Config.DefaultVNCServerHostname;
                        Program.CurrentPort = Config.DefaultVNCServerPort;

                        try
                        {
                            Program.RfbClient.Connect(Config.DefaultVNCServerHostname, Config.DefaultVNCServerPort, _connsettings);
                        }
                        catch (Exception ex)
                        {
                            LogAndBound($"🌋 Connection failed: {ex.Message}");
                        }
                    } else
                    {
                        // this should never happen dumbass
                        LogAndBound($"⚠ Incorrect Configuration{Environment.NewLine}This bot has been configured with {CurrentBoundChannel.Id} as a bound channel, yet it has no VNC server configured, nor does it REQUIRE for its bound channel to be used. The channel will be unbound and the bot will continue.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Log.Error("Bound proceidure fail: " + ex.Message);
                }
            }

            return Task.CompletedTask;
        }

        public static async Task Command_Errored(CommandsNextExtension cmd, CommandErrorEventArgs e)
        {
            string msg = e.Exception.Message;
            Log.Logger.Error(e.Exception.ToString());
            switch (Config.GraphicalError)
            {
                case 1:
                    try
                    {
                        Bitmap _image = new Bitmap(640, 480);
                        Graphics _canvas = Graphics.FromImage(_image);
                        SolidBrush _bluebrush = new SolidBrush(Color.DarkBlue);
                        SolidBrush _whitebrush = new SolidBrush(Color.White);

                        _canvas.Clear(Color.DarkBlue);
                        _canvas.DrawString("A problem has been detected and " + e.Exception.Source + " has shut down the command to prevent damage to the bot." + Environment.NewLine + Environment.NewLine
                            + "The exception seems to have been thrown by the following method: " + e.Exception.TargetSite + Environment.NewLine + Environment.NewLine
                            + e.Exception.Message + Environment.NewLine + Environment.NewLine
                            + "If this is the first time you've seen this Stop error screen, check your arguments and try executing the command again. If this screen appears again, follow these steps:" + Environment.NewLine + Environment.NewLine
                            + "Check to make sure any new package or configuration is properly installed. If this is a new installation, ask the maintainer or hoster for any additional configurations you might need." + Environment.NewLine + Environment.NewLine
                            + "Technical information:" + Environment.NewLine + Environment.NewLine
                            + e.Exception.StackTrace
                            , new Font(FontFamily.GenericMonospace, 12, FontStyle.Regular), _whitebrush, new Rectangle(0, 0, 640, 480));

                        _canvas.Save();

                        MemoryStream _ms = new MemoryStream();
                        _image.Save(_ms, System.Drawing.Imaging.ImageFormat.Png);
                        _ms.Position = 0;

                        DiscordMessageBuilder _msg = new DiscordMessageBuilder()
                            .WithContent($"<@{e.Context.Member.Id}>, **whut ?!**");

                        _msg.AddFile("bsod.png", _ms, true);
                        await _msg.SendAsync(e.Context.Channel);

                        _whitebrush.Dispose();
                        _bluebrush.Dispose();
                        _canvas.Dispose();
                        _image.Dispose();
                        _ms.Dispose();
                    } catch (Exception ex)
                    {
                        await e.Context.RespondAsync($"Exception thrown: ```{msg}```Additional exception thrown while trying to display exception: ```{ex.Message}```");
                    }

                    break;
                case 0:
                    await e.Context.RespondAsync($"Exception thrown: ```{msg}```");
                    break;
            }
        }

        public static void LogAndBound(string msg)
        {
            Log.Information(msg);
            CurrentBoundChannel.SendMessageAsync(msg);
        }

        private static void Vnc_Connection(object sender, EventArgs e)
        {
            Log.Information($"Connection to {CurrentHostname}:{CurrentPort} established.");
            CurrentBoundChannel.SendMessageAsync($"Connection to {CurrentHostname}:{CurrentPort} established.{Environment.NewLine}_By the way, this channel is now bound to notifications and alerts_");
        }

        private static void Vnc_Close(object sender, EventArgs e)
        {
            if (MeantToDisconnect)
            {
                if (!PermissionsChecker.BoundChannelOnly())
                {
                    LogAndBound("Connection Closed." + Environment.NewLine
                                + $"Unbounding bound channel {CurrentBoundChannel.Name}");
                    CurrentBoundChannel = null;
                } else
                {
                    LogAndBound("Connection Closed.");
                }
            }
            else
            {
                if (!PermissionsChecker.BoundChannelOnly())
                {
                    LogAndBound("Connection unexpectedly closed." + Environment.NewLine
                                + $"Unbounding bound channel {CurrentBoundChannel.Name}");
                    CurrentBoundChannel = null;
                }
                else
                {
                    LogAndBound("Connection unexpectedly closed.");
                }
                //LogAndBound("Connection unexpectedly closed.");
                //RfbClient.Connect(CurrentHostname, CurrentPort);
            }
        }

        private static void Vnc_ConnectionFail(object sender, EventArgs e)
        {
            if (!PermissionsChecker.BoundChannelOnly())
            {
                LogAndBound("Connection attempt failed." + Environment.NewLine
                            + $"Unbounding bound channel {CurrentBoundChannel.Name}");
                CurrentBoundChannel = null;
            }
            else
            {
                LogAndBound("Connection attempt failed.");
            }
        }

        private static void Vnc_RemoteClipboardChange(object sender, RemoteClipboardChangedEventArgs e)
        {
            Log.Information("Remote Clipboard change: " + e.Contents);
            DiscordMessageBuilder _dmb = new DiscordMessageBuilder().WithContent("Remote Clipboard change");
            MemoryStream _ms = new MemoryStream();
            byte[] _clipboard = Encoding.UTF8.GetBytes(e.Contents);
            _ms.Write(_clipboard, 0, _clipboard.Length);
            _ms.Position = 0;
            _dmb.AddFile("clipboard.txt", _ms);
            CurrentBoundChannel.SendMessageAsync(_dmb).GetAwaiter().GetResult();
            _ms.Dispose();
        }
    }
}
