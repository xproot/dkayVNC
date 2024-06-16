using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dkayVNC.Commands.Attributes;
using DSharpPlus.Entities;
using System.IO;
using AnimatedGif;
using System.Drawing;
using RemoteViewing.Vnc;
using dkayVNC.Utils;

namespace dkayVNC.Commands
{
    public class KeyCodeCommand : BaseCommandModule
    {
        [Command("keycode")]
        [Aliases(new string[] { "key", "k" })]
        [Description("Send various VNC keycodes. Only one keycode will be held while this plays out.")]
        [Usage("keycode [name, separated by spaces]")]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Cmd(CommandContext ctx, [RemainingText]string keys)
        {
            await ctx.TriggerTypingAsync();

            Program.LastControl = ctx.User.Username;
            Program.LastControlId = ctx.User.Id;
            Program.LastControlTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Program.LastControlType = "Keyboard";

            int keyssent = 0;
            string[] keysyms = keys.Split(' ');

            // This command does not use the Program.cs Gif Encoder cause I wanted to add a frame every key
            MemoryStream _ms = new MemoryStream();
            AnimatedGifCreator gif = new AnimatedGifCreator(_ms, 83);

            foreach (string key in keysyms)
            {
                KeySym keySym;
                if (key.Length == 1 && Char.IsUpper(Convert.ToChar(key)))
                {
                    // I made this thinking it would fix no upper case keys... it didn't??
                    keySym = (KeySym)((int)Convert.ToChar(key));
                    
                    Program.RfbClient.SendKeyEvent(keySym, true);
                    Program.RfbClient.SendKeyEvent(keySym, false);
                    keyssent++;
                } else if (Enum.TryParse(key, true, out keySym))
                {
                    if (Enum.IsDefined(typeof(KeySym), keySym))
                    {
                        Program.RfbClient.SendKeyEvent(keySym, true);
                        Program.RfbClient.SendKeyEvent(keySym, false);
                        keyssent++;

                        gif.AddFrame(Framebuffer.GetRfbBitmap(), -1, GifQuality.Bit8);
                    }
                }
            }

            gif.Dispose();
            _ms.Position = 0;

            if (keyssent > 0)
            {
                DiscordMessageBuilder discordMessageBuilder = new DiscordMessageBuilder().WithContent($"⌨! (keys sent: {keyssent})");
                discordMessageBuilder.AddFile("ss.gif", _ms, false);
                await ctx.RespondAsync(discordMessageBuilder);
            } else
            {
                await ctx.RespondAsync("Lol all of your keys are wrong they didn't convert");
            }
        }
    }
}
