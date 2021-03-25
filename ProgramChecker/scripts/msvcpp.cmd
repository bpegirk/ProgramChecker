@echo off

call "C:\Program Files\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat"
SET COMPILER="C:\Program Files\Microsoft Visual Studio 9.0\VC\bin\cl.exe"

%COMPILER% /Fe%2 /Fo%2 %1
