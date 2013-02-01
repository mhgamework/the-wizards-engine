cd _scripts
call clean.bat
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" "The Wizards.sln" 
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" "The Wizards.sln" 
xcopy /s "The Wizards Gameplay\packages\QuickGraph.3.6.61119.7\lib\net4" "bin\Binaries"
