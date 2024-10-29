pushd $BUILD_SOURCESDIRECTORY
cd Universe.NUnitPipeline.Tests
echo "${TARGET_FRAMEWORKS_TEST:-}" | awk -FFS=";" 'BEGIN{FS=";"}{for(i=1;i<=NF;i++){print $i}}' | while IFS= read -r tf; do
  pushd .; mkdir -p bin; cd bin; mkdir -p public; cd public; outParent=$(pwd); mkdir -p $tf; cd $tf; out=$(pwd); popd
  Say "Building tests for [$tf] v$SELF_VERSION into '$out'"
  dotnet build -c Release -verbosity:quiet -p:PackageVersion=$SELF_VERSION -p:Version=$SELF_VERSION -o $out Universe.NUnitPipeline.Tests.csproj 2>&1 | { grep -v ": warning"; true; }
done
Say "Pack"
7z a -mx=1 "$SYSTEM_ARTIFACTSDIRECTORY/Tests.7z" "bin/public/*"
popd