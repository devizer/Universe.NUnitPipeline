set -eu; set -o pipefail
pushd $BUILD_SOURCESDIRECTORY >/dev/null
cd Universe.NUnitPipeline.Tests
cat *.csproj
count=0; 
error="${HOME}${PATH_SEPARATOR_CHAR}Error-TF.tmp"
touch "$error"
echo "${TARGET_FRAMEWORKS_TEST:-}" | awk -FFS=";" 'BEGIN{FS=";"}{for(i=NF;i>=1;i--){print $i}}' | while IFS= read -r tf; do
  out="bin${PATH_SEPARATOR_CHAR}public${PATH_SEPARATOR_CHAR}$tf"
  count=$((count+1))
  Say "($count) Building tests for [$tf] v$SELF_VERSION into '$out'"
  dotnet build -f $tf -c Release -verbosity:quiet -p:PackageVersion=$SELF_VERSION -p:Version=$SELF_VERSION -o "$out" Universe.NUnitPipeline.Tests.csproj 2>&1 | { grep -v ": warning"; true; } | { grep -v -e '^\s*$'; true; } || { echo "Errrrrrrrrrrrrrrrrrrrrrrooooooooor" | tee -a "$error"; }
  pushd "$out" >/dev/null
    if [[ "$(uname -s)" == Linux ]]; then
      if [[ "$tf" = *"."* ]]; then
        Say --Display-As=Error "Skip NET Core test for [$tf] on linux";
      else
        if [[ "$NUNIT_VERSION" == 3* ]]; then
          time nunit3-console-3.12.0 --noheader --workers=1 --where "cat != Fail" --work=. Universe.NUnitPipeline.Tests.dll
          ls -la TestsOutput
          if [[ -n "${SYSTEM_ARTIFACTSDIRECTORY:-}" ]]; then
            touch "TestsOutput/$tf"
            cp -av TestsOutput "${SYSTEM_ARTIFACTSDIRECTORY:-}"
          fi
        else 
          Say --Display-As=Error "Skip Nunit 4.x NET Framwork tests on linux [$tf]";
        fi
      fi
    fi
  popd >/dev/null
done
errors="$(cat "$error" | wc -l)"
echo "TOTAL ERRORS: $errors"
if [ "$errors" -ne 0 ]; then exit 666; fi
Say "Pack"
cd bin${PATH_SEPARATOR_CHAR}public
7z a -mx=2 -ms=on -mqs=on "$SYSTEM_ARTIFACTSDIRECTORY/Tests-v$SELF_VERSION.7z"
popd >/dev/null
