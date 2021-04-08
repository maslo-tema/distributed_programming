using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryModule
{
    public class RedisStorage : IStorage
    {
        private readonly IConnectionMultiplexer _connection = ConnectionMultiplexer.Connect("localhost, allowAdmin=true");
        private readonly IDatabase _db;
        private readonly IServer _server;

        private readonly IConnectionMultiplexer _serverRus;
        private readonly IConnectionMultiplexer _serverEu;
        private readonly IConnectionMultiplexer _serverOther;
        
        private Dictionary<string, IDatabase> _dateBases;
        public RedisStorage()
        {
            _db = _connection.GetDatabase();
            _server = _connection.GetServer("localhost", 6379);

            _serverRus = ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_RUS", EnvironmentVariableTarget.User));
            _serverEu = ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_EU", EnvironmentVariableTarget.User));
            _serverOther = ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_OTHER", EnvironmentVariableTarget.User));

            _dateBases = GetServers();

        }
        public string GetValue(string key, string id)
        {
            string shardKey = GetShardKey(id);
            return _dateBases[shardKey].StringGet(key);
        }
        public void Add(string key, string value, string id)
        {
            string shardKey = GetShardKey(id);
            _dateBases[shardKey].StringSet(key, value);
        }

        public void AddInSet(string value, string id)
        {;
            string shardKey = GetShardKey(id);
            _dateBases[shardKey].SetAdd("all", value);
        }

        public bool ExistInSet(string value)
        {
            bool exist = false;
            foreach (var db in _dateBases.Values)
            {
                if (db.SetContains("all", value))
                {
                    exist = true;
                }
            }

            return exist;
        }

        private Dictionary<string, IDatabase> GetServers()
        {
            _dateBases = new Dictionary<string, IDatabase>();
            _dateBases.Add("RUS", _serverRus.GetDatabase());
            _dateBases.Add("EU", _serverEu.GetDatabase());
            _dateBases.Add("OTHER", _serverOther.GetDatabase());
            return _dateBases;
        }

        public void AddShardKey(string id, string shardKey)
        {
            _db.StringSet(id, shardKey);
        }

        public string GetShardKey(string id)
        {
            return _db.StringGet(id);
        }
    }
}