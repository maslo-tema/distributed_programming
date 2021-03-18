using System;

namespace Valuator.Events
{
    [Serializable]
    public class EventSimilarity
    {
        public string Id { get; set; }
        public double Similarity { get; set; }
    }
}