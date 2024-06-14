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
    public class ControlCommand : BaseCommandModule
    {
        [Command("lastcontrol")]
        [Aliases(new string[] { "control" })]
        [Description("Shows the last control action done.")]
        [Usage("control")]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Cmd(CommandContext ctx)
        {
            await ctx.RespondAsync($"Connected: {Program.RfbClient.IsConnected}");
        }
    }
}
