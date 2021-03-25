SET COMPILER="C:\Windows\Microsoft.NET\Framework\v4.0.30319\vbc.exe"

%COMPILER% /target:exe /optimize+ /debug- /out:%2 %1 
