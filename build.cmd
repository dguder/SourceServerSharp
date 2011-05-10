@echo off
set FRAMEWORK_PATH=C:/WINDOWS/Microsoft.NET/Framework/v4.0.30319
set PATH=%PATH%;%FRAMEWORK_PATH%;
set ILMERGE_VERSION=v4,%FRAMEWORK_PATH%

msbuild /nologo /verbosity:quiet src/SourceServerSharp.sln /p:Configuration=Release /t:Clean
msbuild /nologo /verbosity:quiet src/SourceServerSharp.sln /p:Configuration=Release

rmdir /s /q output
mkdir output
"lib/ILMerge.exe" /keyfile:src/SssIndex/SsIndex.snk /wildcards /target:winexe /targetplatform:%ILMERGE_VERSION% /out:output/srcindex.exe "src/SssIndex/bin/Release/SssIndex.exe " "src/SssIndex/bin/Release/QQn.SourceServerSharp.dll"
