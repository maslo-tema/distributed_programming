using System;

namespace LibraryModule.EventsModule
{
    [Serializable]
    public class EventSimilarity
    {
        public string Id { get; set; }
        public double Similarity { get; set; }
    }
}