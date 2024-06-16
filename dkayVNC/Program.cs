using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemoteViewing.Vnc;
using Serilog;
using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;
using AnimatedGif;

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

            /*if (clientConfig.DefaultBindToChannel > 0)
            {
                try
                {
                    boundChannel = client.GetChannelAsync(clientConfig.DefaultBindToChannel).GetAwaiter().GetResult();
                    Log.Information($"Binding to channel set in bot config: {boundChannel.Name} ({boundChannel.Id})");
                    boundChannel.SendMessageAsync("This bot has been bound to this channel by bot config. VNC commands in other channels will be ignored.");

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Log.Error("Could not bind to Default Channel: " + ex.Message);
                }
            }

            if (!String.IsNullOrWhiteSpace(clientConfig.DefaultConnectToHost) && clientConfig.DefaultConnectToPort > 0)
            {
                SendToLogAndBoundChannel($"Attempting connection to server set in config: {clientConfig.DefaultConnectToHost}:{clientConfig.DefaultConnectToPort}");
            }*/

            return Task.CompletedTask;
        }

        public static async Task Command_Errored(CommandsNextExtension cmd, CommandErrorEventArgs e)
        {
            string msg = e.Exception.Message;
            Log.Logger.Error(e.Exception.ToString());
            switch (Config.GraphicalError)
            {
                case 1:
                    Bitmap _image = new Bitmap(640, 480);
                    Graphics _canvas = Graphics.FromImage(_image);
                    SolidBrush _bluebrush = new SolidBrush(Color.DarkBlue);
                    SolidBrush _whitebrush = new SolidBrush(Color.White);

                    _canvas.Clear(Color.DarkBlue);
                    _canvas.DrawString("A problem has been detected and "+e.Exception.Source+" has shut down the command to prevent damage to the bot." + Environment.NewLine + Environment.NewLine
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
                LogAndBound("Connection Closed." + Environment.NewLine
                +"Unbounding this channel");
                CurrentBoundChannel = null;
            }
            else
            {
                LogAndBound("Connection unexpectedly closed.");
                //RfbClient.Connect(CurrentHostname, CurrentPort);
            }
            /*Thread.Sleep(100);
            vncclient = new RemoteViewing.Vnc.VncClient();
            Setup();*/
        }

        private static void Vnc_ConnectionFail(object sender, EventArgs e)
        {
            LogAndBound($"Connection attempt failed.");
            /*Thread.Sleep(1000);
            vncclient = new RemoteViewing.Vnc.VncClient();
            Setup();*/
        }

        private static void Vnc_RemoteClipboardChange(object sender, RemoteClipboardChangedEventArgs e)
        {
            LogAndBound("Remote Clipboard change: " + e.Contents);
        }

        public static Bitmap GetRfbBitmap()
        {
            if (RfbClient.IsConnected)
            {
                Bitmap _rfbframebuffer = new Bitmap(RfbClient.Framebuffer.Width, RfbClient.Framebuffer.Height, PixelFormat.Format32bppRgb);

                var _fbrect = new Rectangle(0, 0, RfbClient.Framebuffer.Width, RfbClient.Framebuffer.Height);
                var data = _rfbframebuffer.LockBits(_fbrect, ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                try
                {
                    VncPixelFormat.CopyFromFramebuffer(RfbClient.Framebuffer, new VncRectangle(0, 0, RfbClient.Framebuffer.Width, RfbClient.Framebuffer.Height), data.Scan0, data.Stride, 0, 0);
                }
                finally
                {
                    _rfbframebuffer.UnlockBits(data);
                }

                return _rfbframebuffer;
            }

            Bitmap _novideoframebuffer = new Bitmap(640, 480, PixelFormat.Format32bppArgb);
            Graphics _canvas = Graphics.FromImage(_novideoframebuffer);

            _canvas.DrawString(DateTime.UtcNow.ToString(), new Font(FontFamily.GenericMonospace, 12, FontStyle.Regular), new SolidBrush(Color.White), 0, 0);

            _canvas.DrawString("Not connected", new Font(FontFamily.GenericMonospace, 12, FontStyle.Regular), new SolidBrush(Color.Black), 100, 100);
            _canvas.DrawString("Not connected", new Font(FontFamily.GenericMonospace, 12, FontStyle.Regular), new SolidBrush(Color.White), 99, 99);

            _canvas.DrawString(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(), new Font(FontFamily.GenericMonospace, 12, FontStyle.Regular), new SolidBrush(Color.Green), 200, 200);

            return _novideoframebuffer;
        }

        public static MemoryStream GetRfbMemoryStream(int frames = 0)
        {
            MemoryStream _ms = new MemoryStream();
            if (frames < 1)
            {
                Bitmap _bitmap = Program.GetRfbBitmap();
                _bitmap.Save(_ms, System.Drawing.Imaging.ImageFormat.Png);
                _bitmap.Dispose();
            }    
            else
            {
                AnimatedGifCreator gif = new AnimatedGifCreator(_ms, 83);
                for (int i = 0; i < frames; i++)
                {
                    gif.AddFrameAsync(Program.GetRfbBitmap(), -1, GifQuality.Bit8).GetAwaiter().GetResult();
                }
                gif.Dispose();
            }
            _ms.Position = 0;
            return _ms;
        }
    }
}
