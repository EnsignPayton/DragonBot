using System.Threading.Tasks;
using Discord.WebSocket;
using DragonBot.Utilities;

namespace DragonBot.Media
{
    public class GreeterService
    {
        private readonly IConfigProvider<DragonBotConfig> _configProvider;
        private readonly MediaService _mediaService;

        public GreeterService(IConfigProvider<DragonBotConfig> configProvider, MediaService mediaService)
        {
            _configProvider = configProvider;
            _mediaService = mediaService;
        }

        public async Task OnVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            if (before.VoiceChannel == null && after.VoiceChannel != null)
            {
                await GreetAsync(after.VoiceChannel.Guild.Id);
            }
        }

        public async Task GreetAsync(ulong guildId)
        {
            if (!_mediaService.IsConnected(guildId)) return;

            var config = await _configProvider.GetConfigAsync();
            if (string.IsNullOrWhiteSpace(config.GreetingFile)) return;

            await Task.Delay(1000);
            await _mediaService.PlayFileAsync(config.GreetingFile, guildId);
        }
    }
}
