@echo off
SET COMPILER="C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe"

%COMPILER% /target:exe /optimize+ /debug- /out:%2.exe %1