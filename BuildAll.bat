cd _scripts
call clean.bat
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" "The Wizards.sln" /p:Configuration=Debug /p:Platform=x86
xcopy "bin/Binaries" "bin/BinariesGame" /y
