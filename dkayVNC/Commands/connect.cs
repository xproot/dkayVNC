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
    public class ConnectCommand : BaseCommandModule
    {
        [Command("connect")]
        [Aliases(new string[] { "conn", "server" })]
        [Description("Connect the VNC Client.")]
        [Usage("connect [hostname(Optional if there is a Default)] [port(Optional: 5900)] [password(Optional)]")]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Cmd(CommandContext ctx, string hostname, ushort port = 5900, [RemainingText]string password = "")
        {
            await ctx.TriggerTypingAsync();
            Program.CurrentBoundChannel = ctx.Channel;
            Program.LastControl = ctx.User.Username;
            Program.LastControlId = ctx.User.Id;
            Program.LastControlTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Program.LastControlType = "Connection";
            Program.CurrentHostname = hostname;
            Program.CurrentPort = port;
            Program.RfbClient.Connect(hostname, port);
        }
    }
}
