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
    public class MousePointCommand : BaseCommandModule
    {
        [Command("mousepoint")]
        [Aliases(new string[] { "point", "p", "abs" })]
        [Description("Fixes the cursor to an absolute point.")]
        [Usage("mousepoint [±x] [±y] [drag button: Left/1, Middle/2, Right/3]")]
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
