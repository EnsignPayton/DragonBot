using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace DragonBot.Modules
{
    public abstract class BaseModule : ModuleBase<SocketCommandContext>
    {
        protected Task<IUserMessage> SendEmbedAsync(Func<EmbedBuilder, EmbedBuilder> build) =>
            ReplyAsync(embed: build.Invoke(new EmbedBuilder()).Build());

        protected Task<IUserMessage> SendErrorAsync(string title) =>
            SendEmbedAsync(x => x
                .WithColor(Color.Red)
                .WithTitle(title));

        protected Task<IUserMessage> SendErrorAsync(string title, string message) =>
            SendEmbedAsync(x => x
                .WithColor(Color.Red)
                .WithTitle(title)
                .WithDescription(message));
    }
}
