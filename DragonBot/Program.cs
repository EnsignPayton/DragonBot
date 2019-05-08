using System;
using CommonServiceLocator;
using Discord.Commands;
using Discord.WebSocket;
using DragonStallion.Common.DependencyInjection;
using Topshelf;

namespace DragonBot
{
    internal static class Program
    {
        private static int Main()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            var serviceLocator = BuildServiceLocator();
            var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

            return (int) HostFactory.Run(x =>
            {
                x.Service<Service>(s =>
                {
                    s.ConstructUsing(_ => serviceLocator.GetInstance<Service>());
                    s.WhenStarted(async t => await t.StartAsync(token));
                    s.WhenStopped(async t => await t.StopAsync());
                });

                x.SetServiceName("DragonStallion.DragonBot");
                x.SetDisplayName("Dragon Bot");
                x.SetDescription("Dragon Discord Bot");

                x.UseNLog();

                x.OnException(ex =>
                {
                    logger.Error("Unhandled Exception");
                    logger.Error(ex);
                });
            });
        }

        private static IServiceLocator BuildServiceLocator() =>
            new ServiceLocatorFactory()
                .WithAssemblyTypes(typeof(DiscordSocketClient))
                .WithSingletonTypes(typeof(CommandService), typeof(Random))
                .Build();
    }
}
