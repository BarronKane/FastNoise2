@echo off
setlocal EnableDelayedExpansion

set GETFILE_EXITCODE=0
set URL=%1
set FILEPATH=%2

set argCount=0
for %%x in (%*) do (
    set /A argCount+=1
    set "argVec[!argCount!]=%%~x"
)
if not %argCount%==2 (
    echo TOO MANY ARGUMENTS
    goto Error_Args
)

curl -L "%1" -o "%2"
goto to_exit

:Error_Args
echo.
echo MUST SUPPLY TWO ARGUMENTS:
echo 1, FILE URL
echo 2, FILE PATH
echo.
set GETFILE_EXITCODE=1
goto to_exit

:to_exit
exit /B %GETFILE_EXITCODE%