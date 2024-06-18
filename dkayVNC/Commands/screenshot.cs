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
using AnimatedGif;
using dkayVNC.Utils;

namespace dkayVNC.Commands
{
    public class ScreenshotCommand : BaseCommandModule
    {
        [Command("screenshot")]
        [Aliases(new string[] { "prntscrn", "prntscr", "prntsc", "ss" })]
        [Description("Shows the framebuffer of the vnc client.")]
        [Usage("screenshot [frames(optional)]")]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Cmd(CommandContext ctx, int frames = 0)
        {
            if (!PermissionData.CheckIfAllowed(ctx.Member.Id, ctx.Channel.Id))
            {
                await ctx.RespondAsync("https://http.cat/403");
                return;
            }

            await ctx.TriggerTypingAsync();
            
            DiscordMessageBuilder discordMessageBuilder = new DiscordMessageBuilder().WithContent("📸!");
            
            if (frames > 0)
                discordMessageBuilder.AddFile("ss.gif", Framebuffer.GetRfbMemoryStream(frames), false);
            else
                discordMessageBuilder.AddFile("ss.png", Framebuffer.GetRfbMemoryStream(frames), false);

            await ctx.RespondAsync(discordMessageBuilder);
        }
    }
}
