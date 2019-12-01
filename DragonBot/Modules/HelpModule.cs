using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DragonBot.Utilities;

namespace DragonBot.Modules
{
    public class HelpModule : BaseModule
    {
        [Command("help"), Alias("halp", "h")]
        public Task HelpAsync()
        {
            return SendEmbedAsync(x => x
                .WithColor(Color.Green)
                .WithTitle("Commands List")
                .AddField("Join Voice", "!join".ToCodeBlock())
                .AddField("Leave Voice", "!leave".ToCodeBlock())
                .AddField("List Sound Files", "!listfiles".ToCodeBlock())
                .AddField("Play Sound File", "!play example.ogg".ToCodeBlock())
                .AddField("Roll Dice", "!roll 2d8+4".ToCodeBlock())
                .AddField("Roll Range", "!range 1-100".ToCodeBlock()));
        }
    }
}
