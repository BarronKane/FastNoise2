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

Call :UnZipFile "%2" "%1"

:UnZipFile <ExtractTo> <newzipfile>
set vbs="%temp%\_.vbs"
if exist %vbs% del /f /q %vbs%
>%vbs%  echo Set fso = CreateObject("Scripting.FileSystemObject")
>>%vbs% echo If NOT fso.FolderExists(%1) Then
>>%vbs% echo fso.CreateFolder(%1)
>>%vbs% echo End If
>>%vbs% echo set objShell = CreateObject("Shell.Application")
>>%vbs% echo set FilesInZip=objShell.NameSpace(%2).items
>>%vbs% echo objShell.NameSpace(%1).CopyHere(FilesInZip)
>>%vbs% echo Set fso = Nothing
>>%vbs% echo Set objShell = Nothing
cscript //nologo %vbs%
if exist %vbs% del /f /q %vbs%
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