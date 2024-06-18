using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dkayVNC.Utils;
using System.Security.Cryptography;
using dkayVNC.Commands.Attributes;

namespace dkayVNC.Commands
{
    public class PermissionsCommand : BaseCommandModule
    {
        //TODO: APPROPIATED CODE, PLEASE CLEAN!!
        [Command("permissions")]
        [Aliases(new string[] { "perms", "blacklist", "whitelist", "bl", "wl", "list" })]
        [Description("Sets permissions of who can use the bot. Toggle toggles the enforcement, Togglemode changes the mode of the list (from a whitelist to a blacklist and viceversa)")]
        [Usage("permissions [toggle/t or togglemode/tm OR add/a or delete/del/d] [userid] or `permissions`")]
        public async Task List(CommandContext ctx, string action = "none", ulong id = 0)
        {
            if (action == "toggle" || action == "t")
            {
                if (!PermissionsChecker.IsOwner(ctx.User.Id))
                    throw new Exception("This action requires the user to be an owner.");
                PermissionData.ToggleOn();
                string status = "OFF";
                if (PermissionData.list.Enabled)
                    status = "ON";
                await ctx.RespondAsync($"Toggled the list {status}.");
                return;
            }

            if (action == "togglemode" || action == "tm")
            {
                if (!PermissionsChecker.IsOwner(ctx.User.Id))
                    throw new Exception("This action requires the user to be an owner.");
                PermissionData.ToggleMode();
                string listname = "whitelist";
                if (PermissionData.list.Blacklist)
                    listname = "blacklist";
                await ctx.RespondAsync($"Toggled the list to a {listname}.");
                return;
            }
            if (id > 1000)
            {
                if (!PermissionsChecker.IsOwner(ctx.User.Id))
                    throw new Exception("This action requires the user to be an owner.");
                switch (action)
                {
                    case "add":
                        PermissionData.Add(id);
                        await ctx.RespondAsync($"Added ID {id} to the list.");
                        break;

                    case "a":
                        PermissionData.Add(id);
                        await ctx.RespondAsync($"Added ID {id} to the list.");
                        break;

                    case "delete":
                        PermissionData.Remove(id);
                        await ctx.RespondAsync($"Deleted ID {id} from the list.");
                        break;

                    case "del":
                        PermissionData.Remove(id);
                        await ctx.RespondAsync($"Deleted ID {id} from the list.");
                        break;

                    case "d":
                        PermissionData.Remove(id);
                        await ctx.RespondAsync($"Deleted ID {id} from the list.");
                        break;
                }
            }
            else
            {
                string listname = "Whitelist: ";
                if (PermissionData.list.Blacklist)
                    listname = "Blacklist: ";
                if (PermissionData.list.Enabled)
                    listname += "ON ";
                else
                    listname += "OFF ";

                var embed = new DiscordEmbedBuilder()
                {
                    Title = listname + PermissionData.list.IDs.Count,
                    Color = ctx.Guild.CurrentMember.Color
                };
                string description = "```ansi";
                foreach (ulong uid in PermissionData.list.IDs)
                {
                    description = $"{description}{Environment.NewLine}\x1B[31m{uid}";
                }
                embed.WithDescription(description + "```");
                embed.WithAuthor("dkayVNC", "https://github.com/xproot/dkayVNC", ctx.Client.CurrentUser.AvatarUrl);
                await ctx.RespondAsync(embed);
            }
        }
    }
}
