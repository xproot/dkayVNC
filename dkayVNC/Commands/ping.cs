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
    public class PingCommand : BaseCommandModule
    {
        [Command("ping")]
        [Aliases(new string[] { "pong", "ms" })]
        [Description("Gets the ping between connected services.")]
        [Usage("ping")]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Cmd(CommandContext ctx)
        {
            await ctx.RespondAsync($"🏓 Pong! {Program.Client.Ping}ms");
        }
    }
}
