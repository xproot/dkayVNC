using dkayVNC.Commands.Attributes;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dkayVNC.Commands
{
    public class KeyConvertCommand : BaseCommandModule
    {
        [Command("keyconvert")]
        [Aliases(new string[] { "keyconv", "kc", "kv" })]
        [Description("Converts your input to dkayVNC keycodes.")]
        [Usage("keyconvert [string]")]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Cmd(CommandContext ctx, [RemainingText]string stuff)
        {
            await ctx.RespondAsync($"```{Convert(stuff)}```");
        }

        public Dictionary<char, string> ConvertTo = new Dictionary<char, string>() {
            {' ', "Space"},
            {'!', "(ShiftLeft D1)"},
            {'\'', "Quote"},
            {'"', "(ShiftLeft Quote)"},
            {'#', "(ShiftLeft D3)"},
            {'$', "(ShiftLeft D4)"},
            {'%', "(ShiftLeft D5)"},
            {'&', "(ShiftLeft D7)"},
            {'(', "(ShiftLeft D9)"},
            {')', "(ShiftLeft D0)"},
            {'*', "(ShiftLeft D8)"},
            {'+', "(ShiftLeft Equal)"},
            {',', "Comma"},
            {'-', "Minus"},
            {'.', "Period"},
            {'/', "Slash"},
            {':', "(ShiftLeft Semicolon)"},
            {';', "Semicolon"},
            {'<', "(ShiftLeft Less"},
            {'>', "(ShiftLeft Greater)"},
            {'=', "Equals"},
            {'?', "(ShiftLeft Slash)"},
            {'@', "(ShiftLeft D2)"},
            {'[', "BracketLeft"},
            {']', "BracketRight"},
            {'\\', "Backslash"},
            {'^', "(ShiftLeft D6)"},
            {'_', "(ShiftLeft Minus)"},
            {'`', "Grave"},
            {'{', "(ShiftLeft BracketLeft"},
            {'}', "(ShiftLeft BracketRight"},
            {'|', "(ShiftLeft Backslash)"},
            {'~', "(ShiftLeft Grave)"}
        };

        public string Convert(string input)
        {
            List<string> codes = new List<string>();

            foreach (char ch in input.ToCharArray())
            {
                if (ConvertTo.ContainsKey(ch))
                {
                    codes.Add(ConvertTo[ch]);
                    continue;
                }

                if (Char.IsNumber(ch))
                {
                    codes.Add($"D{ch}");
                    continue;
                }

                if (Char.IsLetter(ch))
                {
                    if (Char.IsUpper(ch))
                        codes.Add($"(ShiftLeft {ch})");
                    else
                        codes.Add(ch.ToString());

                    continue;
                }

                throw new Exception($"Unknown character: {input}");
            }

            return string.Join(" ", codes);
        }
    }
}
