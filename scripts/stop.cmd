taskkill /f /im Valuator.exe
taskkill /f /im redis-server.exe

cd ..\nginx\
nginx -s stop