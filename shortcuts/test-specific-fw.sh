work=$HOME/build/Universe.NUnitPipeline; test ! -d $work && git clone https://github.com/devizer/Universe.NUnitPipeline $work; cd $work
git reset --hard
git clean -xxffd
git pull
cd Universe.NUnitPipeline.Tests 
dotnet add package Microsoft.NETFramework.ReferenceAssemblies
FW=net40
# NET40: error CS1061: 'Task' does not contain a definition for 'GetAwaiter' and no accessible extension method 
Reset-Target-Framework -fw $FW -l latest
dotnet build -c Release -f $FW
rm -rf TestsOutput
nunit3-console bin/Release/$FW/Universe.NUnitPipeline.Tests.dll
cat TestsOutput/*.Tree.Summary.txt 
