using System;
using System.Threading.Tasks;
using CommonServiceLocator;
using Discord;
using Discord.Commands;
using DragonBot.Utilities;
using Newtonsoft.Json;

namespace DragonBot.Modules
{
    [RequireOwner]
    public class AdminModule : BaseModule
    {
        private readonly IConfigProvider<DragonBotConfig> _configProvider;

        public AdminModule(IConfigProvider<DragonBotConfig> configProvider)
        {
            _configProvider = configProvider;
        }

        #region Commands

        [Command("kill", RunMode = RunMode.Async)]
        public async Task KillAsync()
        {
            await SendEmbedAsync(x => x
                .WithColor(Color.DarkGrey)
                .WithTitle("Goodbye"));

            var service = ServiceLocator.Current.GetInstance<Service>();
            await service.StopAsync();
            Environment.Exit(0);
        }

        [Command("config")]
        public async Task GetConfigAsync()
        {
            var config = await _configProvider.GetConfigAsync();
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);

            await SendEmbedAsync(x => x
                .WithColor(Color.DarkGrey)
                .WithTitle("Configuration")
                .WithDescription(json.ToCodeBlock("json")));
        }

        [Command("echo")]
        public async Task EchoAsync(string message)
        {
            await SendEmbedAsync(x => x
                .WithColor(Color.DarkGrey)
                .WithTitle($"{Context.User.Username}:")
                .WithDescription(message.ToCodeBlock()));
        }

        #endregion
    }
}
