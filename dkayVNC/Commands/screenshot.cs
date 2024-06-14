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
using System.Drawing;

namespace dkayVNC.Commands
{
    public class ScreenshotCommand : BaseCommandModule
    {
        [Command("screenshot")]
        [Aliases(new string[] { "prntscrn", "prntscr", "prntsc", "ss" })]
        [Description("Shows the framebuffer of the vnc client.")]
        [Usage("screenshot [frames(optional)]")]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Cmd(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            
            DiscordMessageBuilder discordMessageBuilder = new DiscordMessageBuilder().WithContent("📸!");
            MemoryStream _ms = new MemoryStream();

            discordMessageBuilder.AddFile("ss.png", Program.GetRfbMemoryStream(), false);

            await ctx.RespondAsync(discordMessageBuilder);

            //_ms.Dispose();
        }
    }
}
