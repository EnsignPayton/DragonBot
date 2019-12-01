using System;
using System.Threading.Tasks;
using CommonServiceLocator;
using Discord.Commands;
using Discord.WebSocket;
using DragonBot.Media;
using DragonBot.Utilities;
using Topshelf;

namespace DragonBot
{
    internal static class Program
    {
        private static async Task<int> Main()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            var serviceLocator = BuildServiceLocator();
            var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

            if (string.IsNullOrWhiteSpace(token))
            {
                var configProvider = serviceLocator.GetInstance<IConfigProvider<DragonBotConfig>>();
                var config = await configProvider.GetConfigAsync();
                token = config.DiscordToken;
            }

            return (int) HostFactory.Run(x =>
            {
                x.Service<Service>(s =>
                {
                    s.ConstructUsing(_ => serviceLocator.GetInstance<Service>());
                    s.WhenStarted(async t => await t.StartAsync(token));
                    s.WhenStopped(async t =>
                    {
                        try
                        {
                            await t.StopAsync();
                        }
                        catch (ObjectDisposedException ex)
                        {
                            logger.Warn("Problem stopping service: " + ex);
                        }
                    });
                });

                x.SetServiceName("DragonStallion.DragonBot");
                x.SetDisplayName("Dragon Bot");
                x.SetDescription("Dragon Discord Bot");

                x.UseNLog();

                x.OnException(ex =>
                {
                    logger.Error("Unhandled Exception: " + ex);
                });
            });
        }

        private static IServiceLocator BuildServiceLocator() =>
            new ServiceLocatorFactory()
                .WithAssemblyTypes(typeof(DiscordSocketClient))
                .WithSingletonTypes(
                    typeof(CommandService),
                    typeof(Random),
                    typeof(Service),
                    typeof(MediaService),
                    typeof(GreeterService),
                    typeof(MediaFileService),
                    typeof(FileConfigProvider<DragonBotConfig>))
                .Build();
    }
}
