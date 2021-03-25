@echo off

call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\bin\vcvars32.bat"

SET COMPILER="C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\bin\cl.exe"

%COMPILER% /Fe%2 /Fo%2 %1