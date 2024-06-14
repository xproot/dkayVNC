using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dkayVNC.Commands.Attributes;
using DSharpPlus.Entities;
using System.IO;
using System.Drawing;

namespace dkayVNC.Commands
{
    public class ScreenshotCommand : BaseCommandModule
    {
        [Command("screenshot")]
        [Aliases(new string[] { "prntscrn", "prntscr", "prntsc", "ss" })]
        [Description("Shows the framebuffer of the vnc client.")]
        [Usage("screenshot [frames(optional)]")]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Cmd(CommandContext ctx)
        {
            Console.WriteLine("1");
            await ctx.TriggerTypingAsync();
            
            DiscordMessageBuilder discordMessageBuilder = new DiscordMessageBuilder().WithContent("📸!");
            MemoryStream _ms = new MemoryStream();

            Console.WriteLine("2");

            Bitmap _bitmap = Program.GetRfbBitmap();
            _bitmap.Save(_ms, System.Drawing.Imaging.ImageFormat.Png);
            _bitmap.Dispose();

            Console.WriteLine("3");

            _ms.Position = 0;

            Console.WriteLine("4");

            discordMessageBuilder.AddFile("ss.png", _ms, false);

            Console.WriteLine("5");

            await ctx.RespondAsync(discordMessageBuilder);

            Console.WriteLine("6");
            //_ms.Dispose();
        }
    }
}
