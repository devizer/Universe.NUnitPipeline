msbuild /t:Build /p:Configuration=Release /p:DebugType=Full /verbosity:quiet Universe.NUnitPipeline.Tests.csproj
if x%3x == xFullx (Set where=test=~Tests) ELSE (Set where=test=~ShowAssemblyTargets)
nunit3-console-%2.exe --noheader --workers=1 --where "%where%" --work=bin\Release\%1 bin\Release\%1\Universe.NUnitPipeline.Tests.dll
rem nunit3-console.exe --noheader --workers=1 --where "test =~ /TestsSync/" --work=bin\Release\net35 bin\Release\net35\Universe.NUnitPipeline.Tests.dll