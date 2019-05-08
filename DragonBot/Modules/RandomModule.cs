using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace DragonBot.Modules
{
    public class RandomModule : BaseModule
    {
        private static readonly Regex WhitespaceRegex = new Regex(@"\s+");
        public Random Random { get; set; }

        [Command("roll")]
        public Task RollAsync([Remainder] string text)
        {
            var rolls = WhitespaceRegex
                .Replace(text, string.Empty)
                .Split('+')
                .SelectMany(GetRolls)
                .ToList();

            return SendEmbedAsync(x => x
                .WithColor(Color.Purple)
                .WithTitle($"{Context.User.Username} rolled {rolls.Sum()}")
                .WithDescription($"`{string.Join(" + ", rolls)} -> {rolls.Sum()}`"));
        }

        private IEnumerable<int> GetRolls(string die)
        {
            if (die.Contains('d'))
            {
                var parts = die.Split('d');
                var count = int.Parse(parts[0]);
                var faces = int.Parse(parts[1]);

                for (int i = 0; i < count; i++)
                {
                    yield return Random.Next(1, faces + 1);
                }
            }
            else
            {
                yield return int.Parse(die);
            }
        }

        [Command("range")]
        public Task RangeAsync([Remainder] string text)
        {
            var parts = WhitespaceRegex
                .Replace(text, string.Empty)
                .Split('-')
                .ToList();

            var min = int.Parse(parts[0]);
            var max = int.Parse(parts[1]);
            var result = Random.Next(min, max + 1);

            return SendEmbedAsync(x => x
                .WithColor(Color.Blue)
                .WithTitle($"{Context.User.Username} rolled {result}"));
        }
    }
}
