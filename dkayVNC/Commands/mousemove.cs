using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dkayVNC.Commands.Attributes;

namespace dkayVNC.Commands
{
    public class MouseMoveCommand : BaseCommandModule
    {
        [Command("mousemove")]
        [Aliases(new string[] { "move", "m", "rel" })]
        [Description("Moves the cursor relatively.")]
        [Usage("mousemove [±x] [±y] [drag button: Left/1, Middle/2, Right/3]")]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Cmd(CommandContext ctx)
        {
            Program.LastControl = ctx.User.Username;
            Program.LastControlId = ctx.User.Id;
            Program.LastControlTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Program.LastControlType = "Mouse";

            await ctx.RespondAsync($"todo");
        }
    }
}
