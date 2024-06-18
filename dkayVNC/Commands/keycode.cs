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
using System.Runtime.CompilerServices;

namespace dkayVNC.Commands
{
    public class KeyCodeCommand : BaseCommandModule
    {
        [Command("keycode")]
        [Aliases(new string[] { "key", "k" })]
        [Description("Send various VNC keycodes. Only one keycode will be held while this plays out unless they are inside (this) (ex: `dkay!keycode (ControlLeft AltLeft Delete)`).")]
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
            List<KeySym> heldKeys = new List<KeySym>();
            bool holdKeys = false;

            //// This command does not use the Program.cs Gif Encoder cause I wanted to add a frame every key
            //MemoryStream _ms = new MemoryStream();
            //AnimatedGifCreator gif = new AnimatedGifCreator(_ms, 83);

            foreach (string key in keysyms)
            {
                KeySym keySym;
                string currentKey = key;

                if (key.StartsWith('('))
                {
                    holdKeys = true;
                    currentKey = key.Remove(0, 1);
                }
                if (key.EndsWith(')'))
                {
                    holdKeys = false;
                    currentKey = key.Remove(key.Length - 1);
                }
                if (currentKey.ToLower() == "delay")
                {
                    await Task.Delay(500);
                }

                if (Enum.TryParse(currentKey, true, out keySym))
                {
                    if (Enum.IsDefined(typeof(KeySym), keySym))
                    {
                        Program.RfbClient.SendKeyEvent(keySym, true);
                        if (!holdKeys)
                        {
                            Program.RfbClient.SendKeyEvent(keySym, false);
                            foreach (KeySym keySym2 in heldKeys) { Program.RfbClient.SendKeyEvent(keySym2, false); }
                            heldKeys.RemoveRange(0, heldKeys.Count);
                        }
                        else
                            heldKeys.Add(keySym);

                        await Task.Delay(10);
                        keyssent++;

                        //await gif.AddFrameAsync(Framebuffer.GetRfbBitmap(), -1, GifQuality.Bit8);
                    }
                }
            }
            //gif.AddFrame(Framebuffer.GetRfbBitmap(), -1, GifQuality.Bit8); //in case any activity happens between these frames
            //gif.AddFrame(Framebuffer.GetRfbBitmap(), 2000, GifQuality.Bit8);

            // Stop holding all keys if ) was never specified
            foreach (KeySym keySym in heldKeys)
            {
                Program.RfbClient.SendKeyEvent(keySym, false);
                heldKeys.RemoveRange(0, heldKeys.Count);
            }

            //gif.Dispose();
            //_ms.Position = 0;

            if (keyssent > 0)
            {
                DiscordMessageBuilder discordMessageBuilder = new DiscordMessageBuilder().WithContent($"⌨! (keys sent: {keyssent})");
                discordMessageBuilder.AddFile("ss.gif", Framebuffer.GetRfbMemoryStream(20), false);
                await ctx.RespondAsync(discordMessageBuilder);
            } else
            {
                await ctx.RespondAsync("Lol all of your keys are wrong they didn't convert");
            }
        }
    }
}
