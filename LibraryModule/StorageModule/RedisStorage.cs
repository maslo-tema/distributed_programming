using LibraryModule;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace LibraryModule
{
    public class RedisStorage : IStorage
    {
        private readonly IConnectionMultiplexer _connection = ConnectionMultiplexer.Connect("localhost, allowAdmin=true");
        private readonly IDatabase _db;
        private readonly IServer _server;
        public RedisStorage()
        {
            _db = _connection.GetDatabase();
            _server = _connection.GetServer("localhost", 6379);

        }
        public string GetValue(string key)
        {
            return _db.StringGet(key);
        }
        public void Add(string key, string value)
        {
            _db.StringSet(key, value);
        }

        public void AddInSet(string newKey, string value)
        {
            _db.SetAdd(newKey, value);
        }

        public bool ExistInSet(string newKey, string value)
        {
            bool exist = _db.SetContains(newKey, value);
            return exist;
        }
    }
}