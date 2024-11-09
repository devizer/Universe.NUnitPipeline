set -eu; set -o pipefail
export SKIP_DOTNET_DEPENDENCIES=False
export DOTNET_VERSIONS="2.0 2.1 2.2 3.0 3.1 5.0 6.0 7.0 8.0"
script=https://raw.githubusercontent.com/devizer/test-and-build/master/lab/install-DOTNET.sh; (wget -q -nv --no-check-certificate -O - $script 2>/dev/null || curl -ksSL $script) | bash; test -s /usr/share/dotnet/dotnet && sudo ln -f -s /usr/share/dotnet/dotnet /usr/local/bin/dotnet; test -s /usr/local/share/dotnet/dotnet && sudo ln -f -s /usr/local/share/dotnet/dotnet /usr/local/bin/dotnet; 
cd ../Universe.NUnitPipeline.Tests.Light

for tf in net8.0 net6.0 netcoreapp2.1 netcoreapp2.2 netcoreapp3.0 netcoreapp3.1 net5.0; do
  Say "Publish [$tf] for linux-x64"
  out=bin/public/$tf
  mkdir -p $out
  dotnet publish -c Release --self-contained -r linux-x64 -f $tf -o $out/ || true
  pushd $out
  GZIP=-9 tar czf $SYSTEM_ARTIFACTSDIRECTORY/$tf-NUnit-Pipeline-Tests.tar.gz .
  popd
done
