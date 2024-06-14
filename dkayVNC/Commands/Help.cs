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
        [Aliases(new string[] { "h", "?" })]
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
                        DiscordEmbedBuilder _embed = new DiscordEmbedBuilder()
                        {
                            Title = command.Name,
                            Description = command.Description,
                            Color = ctx.Guild.CurrentMember.Color
                        };
                        if (command.Aliases.Count > 0)
                            _embed.AddField("Aliases", string.Join(", ", command.Aliases), true);
                        else
                            _embed.AddField("Aliases", "_\\*none\\*_");
                        _embed.AddField("Usage", command.CustomAttributes.OfType<UsageAttribute>().FirstOrDefault().Usage, true);
                        
                        await ctx.RespondAsync(_embed);
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
            string cmds = "Total \x1B[0m" + Program.Commands.RegisteredCommands.Count + Environment.NewLine;
            string currentcmd = "none";
            foreach (Command command in Program.Commands.RegisteredCommands.Values)
            {
                if (currentcmd.ToLower() == command.Name.ToLower()) continue;
                /*if (command.Description.Length < 64)
                    cmds = cmds + Environment.NewLine + "\x1B[31m" + command.Name + "\x1B[0m: " + command.Description;
                else
                    cmds = cmds + Environment.NewLine + "\x1B[31m" + command.Name + "\x1B[0m: " + command.Description.Substring(0, 63) + "...";
                */
                cmds = cmds + Environment.NewLine + "\x1B[31m" + command.Name + "\x1B[0m: " + command.Description;
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
