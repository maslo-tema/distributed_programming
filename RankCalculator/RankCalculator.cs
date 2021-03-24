// Reference the NATS client.
using NATS.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Valuator;
using Valuator.Events;

namespace RankCalculator
{
    class RankCalculator
    {
        private readonly IConnection _connection;
        private readonly IAsyncSubscription _subscription;
        private readonly IStorage _storage = new RedisStorage();

        public RankCalculator()
        {
            ConnectionFactory cf = new ConnectionFactory();
            _connection = cf.CreateConnection();
            _subscription = _connection.SubscribeAsync("rank", "rank-calculator", async (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);
                var text = _storage.GetValue("TEXT-" + id);
                string rankKey = "RANK-" + id;
                var rank = RankCalculate(text);
                _storage.Add(rankKey, rank.ToString());

                await PublishEventRankCalculator(id, rank);
            });
        }
        public double RankCalculate(string text)
        {
            if (text != null)
            {
                var countNotLetter = text.Where(x => !(Char.IsLetter(x))).Count();
                double rank = (double)countNotLetter / text.Count();
                return rank;
            }
            else
            {
                return (double)0.0;
            }
        }
        public void RunCount()
        {
            _subscription.Start();
            Console.WriteLine("Press Enter to exit the program");
            Console.ReadLine();
            _subscription.Unsubscribe();
            _connection.Drain();
            _connection.Close();
        }

        private async Task PublishEventRankCalculator(string id, double rank)
        {
            EventRank rankEvent = new EventRank()
            {
                Id = id,
                Rank = rank
            };

            ConnectionFactory cf = new ConnectionFactory();
            using (IConnection c = cf.CreateConnection())
            {
                byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(rankEvent));
                c.Publish("event-rank", data);
                await Task.Delay(1000);
                c.Drain();
                c.Close();
            }
        }
    }
}
