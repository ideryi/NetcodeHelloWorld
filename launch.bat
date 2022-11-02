@echo off

:start
set /p serverOrClient=host press h,server press s,client press c,exit press q:


if "%serverOrClient%"=="h" (
echo choosen host
start NetcodeHelloWorld.exe -logfile log-server.txt -mlapi host 
) else if "%serverOrClient%"=="s" (
echo choosen server
start NetcodeHelloWorld.exe  -logfile log-client.txt -mlapi server
) else if "%serverOrClient%"=="c" (
echo choosen client
start NetcodeHelloWorld.exe  -logfile log-client.txt -mlapi client
) else if "%serverOrClient%" == "q" (
echo choosen exit
goto end
)

goto start


:end