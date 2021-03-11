using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Valuator.Pages
{
    public class SummaryModel : PageModel
    {
        private readonly ILogger<SummaryModel> _logger;
        private readonly IStorage _storage; 

        public SummaryModel(ILogger<SummaryModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public double Rank { get; set; }
        public double Similarity { get; set; }

        public void OnGet(string id)
        {
            _logger.LogDebug(id);

            //TODO: проинициализировать свойства Rank и Similarity сохранёнными в БД значениями
            Rank = GetValueRank(id);
            Similarity = Convert.ToDouble(_storage.GetValue("SIMILARITY-" + id));
        }
        public double GetValueRank(string id)
        {
            int counter, timeWaitText;
            counter = 0;
            timeWaitText = 60;
            TimeSpan delayTime = TimeSpan.FromSeconds(0.01);
            while (counter <= timeWaitText)
            {
                string rankKey = "RANK-" + id;
                string rankString = _storage.GetValue(rankKey);
                if (!String.IsNullOrWhiteSpace(rankString))
                {
                    return Convert.ToDouble(rankString);
                }
                counter++;
                Thread.Sleep(delayTime);
            }
            return (double)0.0;
        }
    }
}
