SET COMPILER="C:\WINDOWS\Microsoft.NET\Framework\v3.5\vbc.exe"

%COMPILER% /target:exe /optimize+ /debug- /out:%2 %1 
