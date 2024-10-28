work=$HOME/build/Universe.NUnitPipeline; git clone https://github.com/devizer/Universe.NUnitPipeline $work; cd $work
git pull
cd Universe.NUnitPipeline.Tests 
dotnet build -c Release -f net48
rm -rf TestsOutput
nunit3-console bin/Release/net48/Universe.NUnitPipeline.Tests.dll
cat TestsOutput/*.