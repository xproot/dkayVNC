using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dkayVNC.Utils;
using dkayVNC.Commands.Attributes;
using RemoteViewing.Vnc;
using DSharpPlus.Entities;

namespace dkayVNC.Commands
{
    public class MousePointCommand : BaseCommandModule
    {
        [Command("mousepoint")]
        [Aliases(new string[] { "point", "p", "abs" })]
        [Description("Fixes the cursor to an absolute point.")]
        [Usage("mousepoint [±x] [±y] [drag button: Left/1, Middle/2, Right/4, ScrollDown/8, ScrollUp/16]")]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Cmd(CommandContext ctx, int x, int y, string button = "none")
        {
            if (!PermissionData.CheckIfAllowed(ctx.Member.Id, ctx.Channel.Id))
            {
                await ctx.RespondAsync("https://http.cat/403");
                return;
            }

            await ctx.TriggerTypingAsync();

            Definitions.MouseButtons mouseButton = Definitions.MouseButtons.None;
            Enum.TryParse(button, true, out mouseButton);

            Program.LastControl = ctx.User.Username;
            Program.LastControlId = ctx.User.Id;
            Program.LastControlTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Program.LastControlType = "Mouse";

            if (mouseButton != Definitions.MouseButtons.None)
            {
                Program.RfbClient.SendPointerEvent(Program.CurrentX, Program.CurrentY, (int)mouseButton);
                await Task.Delay(10);
                Program.RfbClient.SendPointerEvent(x, y, (int)mouseButton);
                await Task.Delay(10);
                Program.RfbClient.SendPointerEvent(x, y, 0);
                await Task.Delay(10);
            }
            else
                Program.RfbClient.SendPointerEvent(x, y, 0);
            Program.CurrentX = x;
            Program.CurrentY = y;

            DiscordMessageBuilder discordMessageBuilder = new DiscordMessageBuilder().WithContent($"kinda todo, ({Program.CurrentX}, {Program.CurrentY}) -> {mouseButton.ToString()} -> ({x}, {y})");

            discordMessageBuilder.AddFile("ss.gif", Framebuffer.GetRfbMemoryStream(20), false);

            await ctx.RespondAsync(discordMessageBuilder);
            //await ctx.RespondAsync($"todo {x},{y},{mouseButton.ToString()}");
        }
    }
}
