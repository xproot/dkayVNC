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
        [Command("permissions")]
        [Aliases(new string[] { "perms", "blacklist", "whitelist", "bl", "wl", "list" })]
        [Description("Sets permissions of who can use the bot. Toggle toggles the enforcement, Togglemode changes the mode of the list (from a whitelist to a blacklist and viceversa)")]
        [Usage("permissions [toggle/t or togglemode/tm OR add/a or delete/del/d] [userid] or `permissions`")]
        public async Task List(CommandContext ctx, string action = "none", ulong id = 0)
        {
            if (!PermissionsChecker.IsOwner(ctx.User.Id))
                throw new Exception("This action requires the user to be an owner.");

            switch (action)
            {
                case "toggle":
                case "t":
                    PermissionData.ToggleOn();

                    string status = PermissionData.list.Enabled ? "ON" : "OFF";
                    await ctx.RespondAsync($"Toggled the list {status}.");
                    return;
                
                case "togglemode":
                case "tm":
                    PermissionData.ToggleMode();
                    
                    string listName = PermissionData.list.Blacklist ? "blacklist" : "whitelist";
                    await ctx.RespondAsync($"Toggled the list to a {listName}.");
                    return;

                case "add":
                case "a":
                    if (id < 1000)
                    {
                        await ctx.RespondAsync($"Invalid ID: {id}");
                        return; 
                    }

                    PermissionData.Add(id);
                    await ctx.RespondAsync($"Added ID {id} to the list.");
                    return;

                case "delete":
                case "del":
                case "d":
                    if (id < 1000)
                    {
                        await ctx.RespondAsync($"Invalid ID: {id}");
                        return; 
                    }
                    
                    PermissionData.Remove(id);
                    await ctx.RespondAsync($"Deleted ID {id} from the list.");
                    return;
            }

            string listname = PermissionData.list.Blacklist ? "Blacklist: " : "Whitelist: ";

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
