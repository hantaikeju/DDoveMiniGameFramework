@echo off
setlocal
set DOTNET_ROLL_FORWARD=LatestMajor
set SCRIPT_DIR=%~dp0
set LUBAN_DLL=%SCRIPT_DIR%Tools\Luban\Luban.dll
set CONF_ROOT=%SCRIPT_DIR%
set REPO_ROOT=%SCRIPT_DIR%..
set CODE_DIR=%REPO_ROOT%\DDoveMiniGameClient\Assets\Game\Generate\Luban
set DATA_DIR=%REPO_ROOT%\DDoveMiniGameClient\Assets\GameRes\Cfg

dotnet "%LUBAN_DLL%" ^
    --conf "%CONF_ROOT%luban.conf" ^
    -t client ^
    -c cs-simple-json ^
    -d json ^
    -x outputCodeDir="%CODE_DIR%" ^
    -x outputDataDir="%DATA_DIR%"
exit /b %ERRORLEVEL%
