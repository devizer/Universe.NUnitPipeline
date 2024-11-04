
function Remove-AllNuGetPackageVersions($PackageId, $ApiKey)
{
    $lower = $PackageId.ToLowerInvariant();

    $json = Invoke-WebRequest -Uri "https://api.nuget.org/v3-flatcontainer/$lower/index.json" | ConvertFrom-Json

    $allVersion = $json.versions;
    $toKeep = @($allVersion | % { "$($_)" } | where { ($_.EndsWith(".$exceptofBuild")) })
    Write-Host "Total Versions: $(@($allVersion).Count)" -ForegroundColor Magenta
    Write-Host "Version to Keep (Count = $($toKeep.Length)): $($toKeep -join "`r`n")" -ForegroundColor Magenta
    Write-Host "$($toKeep -join "`r`n")" -ForegroundColor Magenta

    $toDelete = @($allVersion | % { "$($_)" } | where { (-not $_.EndsWith(".$exceptofBuild")) })
    Write-Host "Version to Delete (Count = $($toDelete.Length)): $($toDelete -join ", ")" -ForegroundColor Magenta

    $index=0;
    foreach($version in $json.versions)
    {
      $Index++
      if ( "$version".EndsWith(".$exceptofBuild") ) { continue; }
      Write-Host "[$index of $($toDelete.Length)] UnListing $PackageId, Ver $version" -ForegroundColor Magenta
      $api="https://api.nuget.org/v3/index.json"
      Invoke-Expression "nuget delete $PackageId $version $ApiKey -source https://api.nuget.org/v3/index.json -NonInteractive"
    }
}

$exceptOfBuild="202"
Remove-AllNuGetPackageVersions "Universe.NUnitPipeline" "$(cat apikey)"