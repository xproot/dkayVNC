using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dkayVNC.Commands.Attributes;
using Serilog;

namespace dkayVNC.Commands
{
    public class DisconnectCommand : BaseCommandModule
    {
        [Command("disconnect")]
        [Aliases(new string[] { "close", "dis", "bye" })]
        [Description("Disconnect the VNC Client.")]
        [Usage("disconnect")]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Cmd(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            Program.MeantToDisconnect = true;
            Program.LastControl = ctx.User.Username;
            Program.LastControlId = ctx.User.Id;
            Program.LastControlTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Program.LastControlType = "Disconnection";

            Log.Logger.Information($"Disconnecting.");
            Program.RfbClient.Close();
        }
    }
}
