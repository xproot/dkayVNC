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

namespace dkayVNC.Commands
{
    public class MousePointCommand : BaseCommandModule
    {
        [Command("mousepoint")]
        [Aliases(new string[] { "point", "p", "abs" })]
        [Description("Fixes the cursor to an absolute point.")]
        [Usage("mousepoint [±x] [±y] [drag button: Left/1, Middle/2, Right/3]")]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Cmd(CommandContext ctx, int x, int y, string button = "none")
        {
            Definitions.MouseButtons mouseButton = Definitions.MouseButtons.None;
            Enum.TryParse(button, true, out mouseButton);

            Program.LastControl = ctx.User.Username;
            Program.LastControlId = ctx.User.Id;
            Program.LastControlTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Program.LastControlType = "Mouse";

            //Program.RfbClient.SendPointerEvent(x, y)

            await ctx.RespondAsync($"todo {x},{y},{mouseButton.ToString()}");
        }
    }
}
