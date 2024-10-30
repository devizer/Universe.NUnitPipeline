pushd $BUILD_SOURCESDIRECTORY >/dev/null
cd Universe.NUnitPipeline.Tests
count=0; 
echo "${TARGET_FRAMEWORKS_TEST:-}" | awk -FFS=";" 'BEGIN{FS=";"}{for(i=NF;i>=1;i--){print $i}}' | while IFS= read -r tf; do
  pushd . >/dev/null; mkdir -p bin; cd bin; mkdir -p public; cd public; outParent=$(pwd); mkdir -p $tf; cd $tf; out=$(pwd); popd >/dev/null
  count=$((count+1))
  Say "($count) Building tests for [$tf] v$SELF_VERSION into '$out'"
  dotnet build -f $tf -c Release -verbosity:quiet -p:PackageVersion=$SELF_VERSION -p:Version=$SELF_VERSION -o $out Universe.NUnitPipeline.Tests.csproj 2>&1 | { grep -v ": warning"; true; } | { grep -v -e '^\s*$'; true; } || { echo "Errrrrrrrrrrrrrrrrrrrrrror" | tee $HOME/Error-TF; }
done
errors="(cat $HOME/Error-TF | wc -l)"
echo "TOTAL ERRORS: $errors"
if [ $errors -ne 0 ]; then exit 666; fi
Say "Pack"
7z a -mx=1 "$SYSTEM_ARTIFACTSDIRECTORY/Tests.7z" "bin/public/*"
popd >/dev/null
