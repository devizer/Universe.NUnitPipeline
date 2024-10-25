msbuild /t:Build /p:Configuration=Release /verbosity:quiet Universe.NUnitPipeline.Tests.csproj
nunit3-console-v3.18.exe --noheader --work=bin\Release\net48 bin\Release\net48\NUnit-Test-Actions.dll