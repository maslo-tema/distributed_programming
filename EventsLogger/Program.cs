using System;

namespace EventsLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new EventsLogger();
            logger.RunLogger();
        }
    }
}
