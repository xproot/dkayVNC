using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dkayVNC.Commands.Attributes;
using DSharpPlus.Entities;

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
            int keyssent = 0;
            string[] keysyms = keys.Split(' ');
            foreach (string key in keysyms)
            {
                RemoteViewing.Vnc.KeySym keySym;
                if (Enum.TryParse(key, out keySym))
                {
                    if (Enum.IsDefined(typeof(RemoteViewing.Vnc.KeySym), keySym))
                    {
                        Program.RfbClient.SendKeyEvent(keySym, true);
                        Program.RfbClient.SendKeyEvent(keySym, false);
                        keyssent++;
                    }
                }
            }
            if (keyssent > 0)
            {
                DiscordMessageBuilder discordMessageBuilder = new DiscordMessageBuilder().WithContent($"⌨! (keys sent: {keyssent})");
                discordMessageBuilder.AddFile("ss.gif", Program.GetRfbMemoryStream(10), false);
                await ctx.RespondAsync(discordMessageBuilder);
            } else
            {
                await ctx.RespondAsync("Lol all of your keys are wrong they didn't convert");
            }
        }
    }
}
