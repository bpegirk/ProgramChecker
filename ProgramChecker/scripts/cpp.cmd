@echo off
path "c:\program files (x86)\codegear\rad studio\5.0\bin\";%path%
set include="Cc:\program files (x86)\codegear\rad studio\5.0\include"
set lib="c:\program files (x86)\codegear\rad studio\5.0\lib"
rem �� ��������� ����� (�������������� ���������� .cpp)
set app="%1"
rem ������ ������� ���������� ���������
if exist %app%.exe del %app%.exe 
if exist %app%.obj del %app%.obj
if exist %app%.tds del %app%.tds
rem ������ ����������
bcc32.exe -I%include% -L%lib% -n%2 %app%
if exist %app%.exe %app%.exe
