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
    public class StatusCommand : BaseCommandModule
    {
        [Command("status")]
        [Aliases(new string[] { "stat"})]
        [Description("Shows the status of the vnc client.")]
        [Usage("status")]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Cmd(CommandContext ctx)
        {
            await ctx.RespondAsync($"lastcontrol blah {Program.LastControlId} id in {Program.LastControlTimestamp} of {Program.LastControlType}");
        }
    }
}
