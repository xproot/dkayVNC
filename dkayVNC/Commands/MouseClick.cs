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
    public class MouseClickCommand : BaseCommandModule
    {
        [Command("mouseclick")]
        [Aliases(new string[] { "click", "c", "clk" })]
        [Description("Clicks the cursor.")]
        [Usage("mouseclick [button: Left/1, Middle/2, Right/4, ScrollDown/8, ScrollUp/16], [count = 1]")]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Cmd(CommandContext ctx, string button = "none", int count = 1)
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

            for (int i = 0; i < count; i++)
            {
                Program.RfbClient.SendPointerEvent(Program.CurrentX, Program.CurrentY, (int)mouseButton);
            }

            DiscordMessageBuilder discordMessageBuilder = new DiscordMessageBuilder().WithContent($"kinda todo, ({Program.CurrentX}, {Program.CurrentY}) -> {mouseButton.ToString()}x{count}");

            discordMessageBuilder.AddFile("ss.gif", Framebuffer.GetRfbMemoryStream(20), false);

            await ctx.RespondAsync(discordMessageBuilder);
            //await ctx.RespondAsync($"todo {x},{y},{mouseButton.ToString()}");
        }
    }
}
