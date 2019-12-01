using System;
using System.Reflection;
using System.Threading.Tasks;
using CommonServiceLocator;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DragonBot.Media;
using DragonBot.Utilities;
using Microsoft.Extensions.Logging;

namespace DragonBot
{
    public class Service : IDisposable
    {
        private readonly ILogger _logger;
        private readonly IConfigProvider<DragonBotConfig> _configProvider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly GreeterService _greeterService;
        private string _messagePrefix;

        public Service(
            ILogger<Service> logger,
            IConfigProvider<DragonBotConfig> configProvider,
            DiscordSocketClient client,
            CommandService commandService,
            GreeterService greeterService)
        {
            _logger = logger;
            _configProvider = configProvider;
            _client = client;
            _commandService = commandService;
            _greeterService = greeterService;

            _client.Log += Client_Log;
            _client.Ready += Client_Ready;
            _client.MessageReceived += Client_MessageReceived;
            _client.UserVoiceStateUpdated += _greeterService.OnVoiceStateUpdated;
        }

        public async Task StartAsync(string token)
        {
            var config = await _configProvider.GetConfigAsync();
            _messagePrefix = config.MessagePrefix;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), ServiceLocator.Current);
        }

        public async Task StopAsync()
        {
            await _client.StopAsync();
            await _client.LogoutAsync();
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        private Task Client_Log(LogMessage message)
        {
            _logger.Log(message);
            return Task.CompletedTask;
        }

        private Task Client_Ready()
        {
            _logger.LogInformation($"Connected with User {_client.CurrentUser.Username}");
            return Task.CompletedTask;
        }

        private async Task Client_MessageReceived(SocketMessage message)
        {
            if (!(message is SocketUserMessage userMessage)) return;
            if (message.Source != MessageSource.User) return;

            _logger.LogDebug($"Received Message ({message.Author.Username} - {message.Channel.Name}): {message}");

            var argPos = 0;
            if (userMessage.HasStringPrefix(_messagePrefix, ref argPos) ||
                userMessage.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, userMessage);
                await _commandService.ExecuteAsync(context, argPos, ServiceLocator.Current);
            }
        }
    }
}
