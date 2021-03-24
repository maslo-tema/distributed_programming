using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LibraryModule;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
// Reference the NATS client.
using NATS.Client;
using LibraryModule.EventsModule;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStorage _storage;

        public IndexModel(ILogger<IndexModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public void OnGet()
        {
            //Console.WriteLine(Process.GetCurrentProcess().Id);
        }
        
        private double CalculateSimilarity(string text)
        {
            if (_storage.ExistInSet("TEXT-", text))
            {
                return (double)1.0;
            }
            else
            {
                return (double)0.0;
            }
        }

        private void CalculateRankInBroker(string id)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Factory.StartNew(async () => await ProduceAsync(id), cts.Token);
        }

        private async Task ProduceAsync(string id)
        {
            ConnectionFactory cf = new ConnectionFactory();

            using (IConnection c = cf.CreateConnection())
            {
                byte[] data = Encoding.UTF8.GetBytes(id);
                c.Publish("rank", data);
                await Task.Delay(1000);

                c.Drain();
                c.Close();
            }
        }

        public IActionResult OnPost(string text)
        {
            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();

            string similarityKey = "SIMILARITY-" + id;
            //TODO: посчитать similarity и сохранить в БД по ключу similarityKey
            double similarity = CalculateSimilarity(text);
            _storage.Add(similarityKey, similarity.ToString());
            PublishEventSimilarityCalculator(id, similarity);

            string textKey = "TEXT-" + id;
            //TODO: сохранить в БД text по ключу textKey
            _storage.Add(textKey, text);
            _storage.AddInSet("TEXT-", text); //сохранить в множество только уникальный текст, который ранее не встречался, для ускорения поиска

            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Factory.StartNew(() => CalculateRankInBroker(id), cts.Token);

            return Redirect($"summary?id={id}");
        }
        private void PublishEventSimilarityCalculator(string id, double similarity)
        {
            EventSimilarity similarityEvent = new EventSimilarity()
            {
                Id = id,
                Similarity = similarity
            };

            ConnectionFactory cf = new ConnectionFactory();
            using (IConnection c = cf.CreateConnection())
            {
                byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(similarityEvent));
                c.Publish("event-similarity", data);
                c.Drain();
                c.Close();
            }
        }
    }
}
