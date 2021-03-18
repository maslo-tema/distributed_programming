using System;

namespace Valuator.Events
{
    [Serializable]
    public class EventRank
    {
        public string Id { get; set; }
        public double Rank { get; set; }
    }
}