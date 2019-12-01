using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using DragonBot.Utilities;

namespace DragonBot.Media
{
    public class MediaFileService : IDisposable
    {
        private readonly string _mediaDirectory;
        private readonly HttpClient _httpClient;

        public MediaFileService(ApplicationSettings settings)
        {
            _mediaDirectory = settings.MediaDirectory;
            _httpClient = new HttpClient();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        public string GetFullPath(string file)
        {
            return Path.Combine(_mediaDirectory, file);
        }

        public string FindFile(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("No source specified");

            var files = ListFiles().ToList();

            // Try exact file first
            if (files.Contains(text))
            {
                return text;
            }

            // Next, indexed file
            if (int.TryParse(text, out var index) && files.Count >= index)
            {
                var file = files[index - 1];
                return file;
            }

            // Next, approximate file name
            var approxFile = files.FirstOrDefault(x => Path.GetFileName(x)
                .StartsWith(text, StringComparison.OrdinalIgnoreCase));

            if (approxFile != null)
            {
                return approxFile;
            }

            throw new ArgumentException("No matching file found");
        }

        public IEnumerable<string> ListFiles()
        {
            if (!Directory.Exists(_mediaDirectory))
                return Enumerable.Empty<string>();

            return Directory.GetFiles(_mediaDirectory)
                .Select(Path.GetFileName);
        }

        public async Task DownloadFileAsync(IMessage message)
        {
            if (message.Attachments == null || !message.Attachments.Any())
                throw new ArgumentException("No attachments found");

            if (message.Attachments.Any(attachment => string.IsNullOrWhiteSpace(attachment.Filename) ||
                                                      string.IsNullOrWhiteSpace(attachment.Url)))
                throw new ArgumentException("Malformed name or URL for attachment");

            foreach (var attachment in message.Attachments)
            {
                var filePath = GetFullPath(attachment.Filename);
                if (File.Exists(filePath))
                    throw new InvalidOperationException($"File {attachment.Filename} already exists");

                var fileData = await _httpClient.GetByteArrayAsync(attachment.Url);
                await File.WriteAllBytesAsync(filePath, fileData);
            }
        }
    }
}
