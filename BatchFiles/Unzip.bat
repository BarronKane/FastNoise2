@echo off
setlocal EnableDelayedExpansion

set UNZIP_EXITCODE=0
set FILEPATH=%1
set TARGETPATH=%2

set argCount=0
for %%x in (%*) do (
    set /A argCount+=1
    set "argVec[!argCount!]=%%~x"
)
if not %argCount%==2 (
    echo TOO MANY ARGUMENTS
    goto Error_Args
)

tar -xf "%1" -C "%2"
goto to_exit

:Error_Args
echo.
echo MUST SUPPLY TWO ARGUMENTS:
echo 1, FILE PATH
echo 2, TARGET PATH
echo.
set UNZIP_EXITCODE=1
goto to_exit

:to_exit
exit /B %UNZIP_EXITCODE%