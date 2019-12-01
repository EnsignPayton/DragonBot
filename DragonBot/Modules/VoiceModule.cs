using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DragonBot.Media;
using DragonBot.Utilities;
using Microsoft.Extensions.Logging;

namespace DragonBot.Modules
{
    public class VoiceModule : BaseModule
    {
        private readonly ILogger _logger;
        private readonly MediaService _mediaService;
        private readonly MediaFileService _mediaFileService;

        public VoiceModule(ILogger<VoiceModule> logger, MediaService mediaService, MediaFileService mediaFileService)
        {
            _logger = logger;
            _mediaService = mediaService;
            _mediaFileService = mediaFileService;
        }

        private ulong GuildId => Context.Guild.Id;

        #region Commands

        [Command("join", RunMode = RunMode.Async), Alias("j")]
        public async Task JoinChannelAsync(IVoiceChannel channel = null)
        {
            var voiceChannel = channel ??
                               (Context.User as IGuildUser)?.VoiceChannel ??
                               Context.Guild.VoiceChannels.FirstOrDefault();

            if (voiceChannel == null)
            {
                await SendErrorAsync("No voice channels available.");
                return;
            }

            await _mediaService.ConnectAsync(voiceChannel, GuildId);
        }

        [Command("leave", RunMode = RunMode.Async)]
        public async Task LeaveAsync()
        {
            await _mediaService.DisconnectAsync(GuildId);
            _logger.LogInformation("Left voice");
        }

        [Command("listfiles"), Alias("listsongs", "listmedia", "list", "l")]
        public async Task ListFilesAsync()
        {
            var files = _mediaFileService.ListFiles().ToList();

            if (files.Any())
            {
                await SendEmbedAsync(x => x
                    .WithColor(Color.DarkPurple)
                    .WithTitle("Media Files")
                    .WithDescription(GetFileListBlock(files)));
            }
            else
            {
                await SendErrorAsync("No media files found");
            }
        }

        [Command("play", RunMode = RunMode.Async), Alias("p")]
        public async Task PlayAsync(string file)
        {
            try
            {
                await _mediaService.PlayAsync(file, GuildId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Playing media: {ex}");
                await SendErrorAsync("Error playing media", ex.Message);
            }
        }

        [Command("upload", RunMode = RunMode.Async)]
        public async Task UploadAsync()
        {
            try
            {
                await _mediaFileService.DownloadFileAsync(Context.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Downloading media: {ex}");
                await SendErrorAsync("Error downloading media", ex.Message);
            }
        }

        #endregion

        #region Methods

        private static string GetFileListBlock(IList<string> files)
        {
            return string.Join(Environment.NewLine, GetFileLines(files)).ToCodeBlock("cs");
        }

        private static IEnumerable<string> GetFileLines(IList<string> files)
        {
            var lineHeight = (int)Math.Ceiling(files.Count / 2.0);

            for (int i = 0; i < lineHeight; i++)
            {
                var iRight = i + lineHeight;
                yield return GetFileLine(i, files[i]) +
                             (files.Count > iRight ? GetFileLine(iRight, files[iRight]) : "");
            }
        }

        private static string GetFileLine(int index, string fileName)
        {
            return fileName != null ? $"{index + 1,-3} {fileName,-25} " : "";
        }

        #endregion
    }
}
