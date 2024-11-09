for %%f in (netcoreapp2.1 netcoreapp2.2 netcoreapp3.0 netcoreapp3.1) DO (
  msbuild /t:restore Universe.NUnitPipeline.Tests.Light.csproj 
  dotnet publish --self-contained -r linux-x64 -f %%f -v:q
)