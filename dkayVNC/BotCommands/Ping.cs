using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dkayVNC.BotCommands
{
    public class PingCommand : BaseCommandModule
    {
        [Command("ping")]
        [Aliases(new string[] { "p", "pong" })]
        [Description("Gets the ping between connected services.")]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task GreetCommand(CommandContext ctx)
        {
            await ctx.RespondAsync("🏓 Pong!" + Environment.NewLine +
                                   $"Discord WS: {Bot.client.Ping}ms");
        }
    }
}
