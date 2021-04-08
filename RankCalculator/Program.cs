using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;

namespace RankCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder => {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });
            var rank = new RankCalculator(new Logger<RankCalculator>(loggerFactory));
            rank.RunCount();
        }
    }
}
