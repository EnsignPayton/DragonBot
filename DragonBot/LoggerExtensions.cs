using System;
using Discord;
using Microsoft.Extensions.Logging;

namespace DragonBot
{
    public static class LoggerExtensions
    {
        public static void Log(this ILogger logger, LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                    logger.LogCritical(message.ToString());
                    break;
                case LogSeverity.Error:
                    logger.LogError(message.ToString());
                    break;
                case LogSeverity.Warning:
                    logger.LogWarning(message.ToString());
                    break;
                case LogSeverity.Info:
                    logger.LogInformation(message.ToString());
                    break;
                case LogSeverity.Verbose:
                    logger.LogDebug(message.ToString());
                    break;
                case LogSeverity.Debug:
                    logger.LogTrace(message.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
