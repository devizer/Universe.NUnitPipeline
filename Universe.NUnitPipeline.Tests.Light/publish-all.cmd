rem for %%f in (netcoreapp2.1 netcoreapp2.2 netcoreapp3.0 netcoreapp3.1) DO (
for %%f in (net6.0 net8.0) DO (
  msbuild /t:restore Universe.NUnitPipeline.Tests.Light.csproj 
  dotnet publish -c Release --self-contained -r linux-x64 -f %%f -v:q
)