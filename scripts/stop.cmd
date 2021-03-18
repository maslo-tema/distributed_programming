taskkill /f /im Valuator.exe
taskkill /f /im RankCalculator.exe
taskkill /f /im EventsLogger.exe

cd ..\nginx\
nginx -s stop