using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LibraryModule.EventsModule;
using NATS.Client;

namespace EventsLogger
{
    public class EventsLogger
    {
        private IConnection _connection = new ConnectionFactory().CreateConnection();
        private IAsyncSubscription _subscription;

        public void RunLogger()
        {
            _subscription.Start();
            Console.WriteLine("Press Enter to exit the program");
            Console.ReadLine();
            _subscription.Unsubscribe();
            _connection.Drain();
            _connection.Close();
        }

        public EventsLogger()
        {
            PrintInformationAboutEvents();
        }

        private void PrintInformationAboutEvents()
        {
            _subscription = _connection.SubscribeAsync("event-rank", (sender, args) =>
            {
                Console.WriteLine($"Event: {args.Message.Subject}");

                EventRank rank = JsonSerializer.Deserialize<EventRank>(args.Message.Data);

                Console.WriteLine($"Id: {rank.Id}");
                Console.WriteLine($"Rank: {rank.Rank}");
                Console.WriteLine();
            });

            _subscription = _connection.SubscribeAsync("event-similarity", (sender, args) =>
            {
                Console.WriteLine($"Event: {args.Message.Subject}");

                EventSimilarity similarity = JsonSerializer.Deserialize<EventSimilarity>(args.Message.Data);

                Console.WriteLine($"Id: {similarity.Id}");
                Console.WriteLine($"Similarity: {similarity.Similarity}");
                Console.WriteLine();
            });
        }
    }
}
