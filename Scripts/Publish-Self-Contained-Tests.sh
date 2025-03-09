set -eu; set -o pipefail

function publish() {
  tf=$1
  dotnet=$2
  oldPATH="$PATH"
  export SKIP_DOTNET_DEPENDENCIES=False
  export DOTNET_VERSIONS="$dotnet"
  export DOTNET_TARGET_DIR="$HOME/dotnet/$dotnet"
  Say "Install dotnet [$dotnet] to [$DOTNET_TARGET_DIR]"
  script=https://raw.githubusercontent.com/devizer/test-and-build/master/lab/install-DOTNET.sh; (wget -q -nv --no-check-certificate -O - $script 2>/dev/null || curl -ksSL $script) | bash; test -s /usr/share/dotnet/dotnet && sudo ln -f -s /usr/share/dotnet/dotnet /usr/local/bin/dotnet; test -s /usr/local/share/dotnet/dotnet && sudo ln -f -s /usr/local/share/dotnet/dotnet /usr/local/bin/dotnet; 

  export PATH="$DOTNET_TARGET_DIR:$PATH"
  dotnet --info

  Say "Publish [$tf] for linux-x64"
  out=bin/public/$tf
  mkdir -p $out
  pushd ..
    Reset-Target-Framework -fw $tf -l latest
  popd
  dotnet publish -c Release --self-contained -r linux-x64 -f $tf -o $out/ || true
  pushd $out
  GZIP=-9 tar czf $SYSTEM_ARTIFACTSDIRECTORY/$tf-NUnit-Pipeline-Tests.tar.gz .
  popd
  
  export PATH="$oldPATH"
}


cd ../Universe.NUnitPipeline.Tests.Light
publish net8.0          8.0
publish netcoreapp2.0   2.0
publish netcoreapp2.1   2.1
publish netcoreapp2.2   2.2
publish netcoreapp3.0   3.0
publish netcoreapp3.1   3.1
publish net7.0          7.0
publish net6.0          6.0
publish net5.0          5.0

