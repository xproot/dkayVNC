using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dkayVNC.Commands.Attributes;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace dkayVNC.Commands
{
    public class HelpCommand : BaseCommandModule
    {
        [Command("help")]
        [Description("there is no more help, you cannot run.")]
        [Usage("help [command(optional)]")]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Help(CommandContext ctx, [RemainingText] string txt)
        {
            if (txt != null)
            {
                foreach (Command command in Program.Commands.RegisteredCommands.Values)
                {
                    if (command.Name.ToLower() == txt.ToLower() || command.Aliases.Contains(txt.ToLower()))
                    {
                        await ctx.RespondAsync(new DiscordEmbedBuilder()
                        {
                            Title = command.Name,
                            Description = command.Description,
                            Color = ctx.Guild.CurrentMember.Color
                        });
                        return;
                    }
                    continue;
                }
                if (txt.ToLower().EndsWith("er"))
                {
                    await ctx.RespondAsync(txt.Replace('@', ' ') + "? I hardly know her!");
                    return;
                }
                if (txt.ToLower() == "ass")
                {
                    await ctx.RespondAsync("😳");
                    return;
                }
                throw new Exception("I HAVE NO IDEA WHAT IS THAT");
            }
            string cmds = "Total " + Program.Commands.RegisteredCommands.Count + Environment.NewLine;
            string currentcmd = "none";
            foreach (Command command in Program.Commands.RegisteredCommands.Values)
            {
                if (currentcmd.ToLower() == command.Name.ToLower()) continue;
                if (command.Description.Length < 36)
                    cmds = cmds + Environment.NewLine + "\x1B[31m" + command.Name + "\x1B[0m: " + command.Description;
                else
                    cmds = cmds + Environment.NewLine + "\x1B[31m" + command.Name + "\x1B[0m: " + command.Description.Substring(0, 35) + "...";
                currentcmd = command.Name;
            }

            await ctx.RespondAsync(new DiscordEmbedBuilder()
            {
                Title = "Directory Listing",
                Description = $"```ansi{Environment.NewLine + cmds}```",
                Color = ctx.Guild.CurrentMember.Color
            });
        }
    }
}
