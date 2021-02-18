using System;
using System.Collections.Generic;
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

        }

        public IActionResult OnPost(string text)
        {
            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();

            string similarityKey = "SIMILARITY-" + id;
            //TODO: посчитать similarity и сохранить в БД по ключу similarityKey
            if (_storage.Exist("TEXT-", text))
            {
                _storage.Add(similarityKey, "1");
            }
            else
            {
                _storage.Add(similarityKey, "0");
            }

            string textKey = "TEXT-" + id;
            //TODO: сохранить в БД text по ключу textKey
            _storage.Add(textKey, text);

            string rankKey = "RANK-" + id;
            //TODO: посчитать rank и сохранить в БД по ключу rankKey
            var countLetter = text.Where(x => Char.IsLetter(x)).Count();
            double rank = (double)countLetter / text.Count();
            _storage.Add(rankKey, rank.ToString());

            return Redirect($"summary?id={id}");
        }
    }
}
