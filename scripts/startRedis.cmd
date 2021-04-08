setx DB_RUS "localhost:6000"
setx DB_EU "localhost:6001"
setx DB_OTHER "localhost:6002"

cd "C:\Program Files\Redis\"
start "RedisServer" redis-server
start "RusServer" redis-server --port 6000
start "EuServer" redis-server --port 6001
start "OtherServer" redis-server --port 6002