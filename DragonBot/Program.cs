using System;
using System.Threading.Tasks;
using CommonServiceLocator;
using Discord.WebSocket;
using DragonStallion.Common.ServiceLocation;

namespace DragonBot
{
    internal static class Program
    {
        private static async Task Main()
        {
            var serviceLocator = BuildServiceLocator();
            var service = serviceLocator.GetInstance<Service>();

            await service.StartAsync(Environment.GetEnvironmentVariable("DISCORD_TOKEN"));

            // Wait for escape
            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
            }

            await service.StopAsync();
            service.Dispose();
        }

        private static IServiceLocator BuildServiceLocator() =>
            new ServiceLocatorFactory()
                .WithAssemblyTypes(typeof(DiscordSocketClient))
                .Build();
    }
}
