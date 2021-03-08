using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

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
        
        private double CalculateRank(string text)
        {
            if (text != null)
            {
                var countNotLetter = text.Where(x => !(Char.IsLetter(x))).Count();
                double rank = (double)countNotLetter / text.Count();
                return rank;
            }
            else
            {
                return 0;
            }
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


        public IActionResult OnPost(string text)
        {
            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();

            string similarityKey = "SIMILARITY-" + id;
            //TODO: посчитать similarity и сохранить в БД по ключу similarityKey
            double similarity = CalculateSimilarity(text);
            _storage.Add(similarityKey, similarity.ToString());

            string textKey = "TEXT-" + id;
            //TODO: сохранить в БД text по ключу textKey
            _storage.Add(textKey, text);
            _storage.AddInSet("TEXT-", text); //сохранить в множество только уникальный текст, который ранее не встречался, для ускорения поиска

            string rankKey = "RANK-" + id;
            //TODO: посчитать rank и сохранить в БД по ключу rankKey
            //rank - доля НЕалфавитных символов в тексте
            double rank = CalculateRank(text);
            _storage.Add(rankKey, rank.ToString());

            return Redirect($"summary?id={id}");
        }
    }
}
