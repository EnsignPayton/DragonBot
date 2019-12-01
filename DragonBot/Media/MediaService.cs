using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Logging;

namespace DragonBot.Media
{
    public class MediaService
    {
        private readonly IDictionary<ulong, GuildMediaPlayer> _players;
        private readonly ILogger _logger;
        private readonly MediaFileService _mediaFileService;

        public MediaService(ILogger<MediaService> logger, MediaFileService mediaFileService)
        {
            _players = new Dictionary<ulong, GuildMediaPlayer>();
            _logger = logger;
            _mediaFileService = mediaFileService;
        }

        public bool IsConnected(ulong guildId)
        {
            return _players.TryGetValue(guildId, out var player) &&
                   player.IsConnected;
        }

        public async Task DisconnectAsync(ulong guildId)
        {
            if (_players.TryGetValue(guildId, out var player))
            {
                await player.DisposeAsync();
                _players.Remove(guildId);
            }
        }

        public async Task ConnectAsync(IVoiceChannel voiceChannel, ulong guildId)
        {
            if (_players.ContainsKey(guildId))
            {
                await DisconnectAsync(guildId);
            }

            var audioClient = await voiceChannel.ConnectAsync();
            _players.Add(guildId, new GuildMediaPlayer(_mediaFileService, audioClient));

            _logger.LogInformation($"Joined Voice Channel {voiceChannel.Name} ({guildId})");
        }

        public async Task PlayAsync(string message, ulong guildId)
        {
            if (!IsConnected(guildId)) throw new InvalidOperationException("Voice disconnected");
            var file = _mediaFileService.FindFile(message);
            await _players[guildId].PlayFileAsync(file);
        }

        public async Task PlayFileAsync(string file, ulong guildId)
        {
            if (!IsConnected(guildId)) throw new InvalidOperationException("Voice disconnected");
            await _players[guildId].PlayFileAsync(file);
        }
    }
}
