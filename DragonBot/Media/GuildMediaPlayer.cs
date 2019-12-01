using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using DragonBot.Utilities;
using NAudio.Wave;

namespace DragonBot.Media
{
    public class GuildMediaPlayer : IAsyncDisposable, IDisposable
    {
        private readonly MediaFileService _mediaFileService;
        private readonly IAudioClient _audioClient;

        public GuildMediaPlayer(MediaFileService mediaFileService, IAudioClient audioClient)
        {
            _mediaFileService = mediaFileService;
            _audioClient = audioClient;
        }

        public bool IsConnected => _audioClient.ConnectionState == ConnectionState.Connected;

        public async ValueTask DisposeAsync()
        {
            if (IsConnected)
                await _audioClient.StopAsync();

            Dispose();
        }

        public void Dispose()
        {
            _audioClient.Dispose();
        }

        public async Task PlayFileAsync(string file)
        {
            if (!IsConnected) throw new InvalidOperationException("Voice disconnected");
            await PlayThroughNAudioAsync(_mediaFileService.GetFullPath(file));
        }

        private async Task PlayThroughNAudioAsync(string file)
        {
            await using var fileReader = new VorbisWaveReader(file);

            var waveFormat = new WaveFormat(48000, 16, fileReader.WaveFormat.Channels);
            using var resampler = new MediaFoundationResampler(fileReader, waveFormat);
            await using var facade = new StreamFacade(resampler);

            await PlayStreamAsync(facade);
        }

        private async Task PlayStreamAsync(Stream stream)
        {
            await using var discord = _audioClient.CreatePCMStream(AudioApplication.Mixed);
            await _audioClient.SetSpeakingAsync(true);
            await stream.CopyToAsync(discord);
            await discord.FlushAsync();
            await _audioClient.SetSpeakingAsync(false);
        }

        private class StreamFacade : Stream
        {
            private readonly IWaveProvider _waveProvider;

            public StreamFacade(IWaveProvider waveProvider)
            {
                _waveProvider = waveProvider;
            }

            public override void Flush()
            {
                throw new NotImplementedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return _waveProvider.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotImplementedException();
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }

            public override bool CanRead { get; } = true;
            public override bool CanSeek { get; } = false;
            public override bool CanWrite { get; } = false;
            public override long Length { get; } = 0;
            public override long Position { get; set; }
        }
    }
}
