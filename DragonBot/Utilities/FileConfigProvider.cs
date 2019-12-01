using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DragonBot.Utilities
{
    public class FileConfigProvider<T> : IConfigProvider<T>
    {
        private static readonly string _localPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.json");

        private readonly ILogger _logger;
        private readonly ApplicationSettings _configuration;

        public FileConfigProvider(
            ILogger<FileConfigProvider<T>> logger,
            ApplicationSettings configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<T> GetConfigAsync()
        {
            EnsureConfigDirectory();
            EnsureConfigFile();

            var json = await File.ReadAllTextAsync(_configuration.ConfigPath);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task SaveConfigAsync(T config)
        {
            EnsureConfigDirectory();

            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            await File.WriteAllTextAsync(_configuration.ConfigPath, json);
        }

        private void EnsureConfigDirectory()
        {
            var configFolder = Directory.GetParent(_configuration.ConfigPath).FullName;

            if (!Directory.Exists(configFolder))
                Directory.CreateDirectory(configFolder);
        }

        private void EnsureConfigFile()
        {
            if (!File.Exists(_configuration.ConfigPath))
                File.Copy(_localPath, _configuration.ConfigPath);
        }
    }
}
