using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace DragonBot.Modules
{
    public class HelpModule : BaseModule
    {
        [Command("help")]
        public Task HelpAsync()
        {
            return SendEmbedAsync(x => x
                .WithColor(Color.Green)
                .WithTitle("Commands List")
                .AddField("Roll Dice", @"```
!roll 2d8+4
```")
                .AddField("Roll Range", @"```
!range 1-100
```"));
        }
    }
}
