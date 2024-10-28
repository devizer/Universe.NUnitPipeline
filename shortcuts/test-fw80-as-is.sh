work=$HOME/build/Universe.NUnitPipeline; git clone https://github.com/devizer/Universe.NUnitPipeline $work; cd $work
git reset --hard; git clean -xxffd; git pull
cd Universe.NUnitPipeline.Tests 
dotnet test -c Release -f net8.0 --logger "console;verbosity=normal;consoleLoggerParameters=ErrorsOnly" -verbosity:minimal 
find . -name '*.Tree.Summary.txt' -exec cat {} \;
