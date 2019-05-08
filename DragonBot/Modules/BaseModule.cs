using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace DragonBot.Modules
{
    public abstract class BaseModule : ModuleBase<SocketCommandContext>
    {
        protected Task SendEmbedAsync(Func<EmbedBuilder, EmbedBuilder> build) =>
            ReplyAsync(embed: build.Invoke(new EmbedBuilder()).Build());
    }
}
