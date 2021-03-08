cd  ..\Redis\
start redis-server.exe

cd ..\Valuator\
start dotnet run --no-build --urls "http://localhost:5001"
start dotnet run --no-build --urls "http://localhost:5002"

cd ..\nginx\
start nginx.exe