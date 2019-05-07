using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DragonBot
{
    public class Service : IDisposable
    {
        private readonly DiscordSocketClient _client;

        public Service(DiscordSocketClient client)
        {
            _client = client;
            _client.Log += Client_Log;
            _client.Ready += Client_Ready;
            _client.MessageReceived += Client_MessageReceived;
        }

        public async Task StartAsync(string token)
        {
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
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
            return Console.Out.WriteLineAsync(message.ToString());
        }

        private Task Client_Ready()
        {
            return Console.Out.WriteLineAsync($"Connected with User {_client.CurrentUser.Username}");
        }

        private async Task Client_MessageReceived(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id) return;

            await Console.Out.WriteLineAsync(message.ToString());
            await Task.CompletedTask;
        }
    }
}
