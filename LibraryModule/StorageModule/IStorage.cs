using System.Collections.Generic;

namespace LibraryModule
{
    public interface IStorage
    {
        void Add(string key, string value, string id);

        void AddShardKey(string id, string shardKey);

        string GetValue(string key, string id);
        
        string GetShardKey(string id);

        void AddInSet(string value, string id);

        bool ExistInSet(string value);
    }
}
