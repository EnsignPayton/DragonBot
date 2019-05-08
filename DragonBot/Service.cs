using System;
using System.Reflection;
using System.Threading.Tasks;
using CommonServiceLocator;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace DragonBot
{
    public class Service : IDisposable
    {
        private const char MessagePrefix = '!';

        private readonly ILogger _logger;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;

        public Service(ILogger<Service> logger, DiscordSocketClient client, CommandService commandService)
        {
            _logger = logger;
            _client = client;
            _commandService = commandService;

            _client.Log += Client_Log;
            _client.Ready += Client_Ready;
            _client.MessageReceived += Client_MessageReceived;
        }

        public async Task StartAsync(string token)
        {
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

            _logger.LogDebug($"Received Message: {message}");

            var argPos = 0;
            if (!userMessage.HasCharPrefix(MessagePrefix, ref argPos)) return;

            var context = new SocketCommandContext(_client, userMessage);
            await _commandService.ExecuteAsync(context, argPos, ServiceLocator.Current);
        }
    }
}
