using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace Valuator
{
    public class RedisStorage: IStorage
    {
        private readonly ILogger<RedisStorage> _logger;
        private readonly IConnectionMultiplexer _connection;
        private readonly IDatabase _db;
        public RedisStorage(ILogger<RedisStorage> logger)
        {
            _logger = logger;
            _connection = ConnectionMultiplexer.Connect("localhost, allowAdmin=true");
            _db = _connection.GetDatabase();

        }
        public string GetValue(string key)
        {
            return _db.StringGet(key);
        }
        public void Add(string key, string value)
        {
            _db.StringSet(key, value);
        }

        public bool Exist(string prefix, string value)
        {
            var server = _connection.GetServer("localhost", 6379);
            var values = server.Keys(pattern: "*" + prefix + "*").Select(x => GetValue(x)).ToList();
            bool exist = values.Exists(x => x == value);
            return exist;
        }
    }
}