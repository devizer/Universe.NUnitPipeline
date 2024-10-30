foreach($ext in "nupkg", "snupkg") {
  $files = @(Get-ChildItem -Path "." -Filter "*.$ext" | Sort -Property FullName)
  $i = 0;
  foreach($file in $files) {
    $i++
    $arguments = @("push", "-Apikey", "$(cat apikey)", "-Timeout", "600", "-Source", "https://www.nuget.org/api/v2/package", "$file")
    Write-Host "[$i of $($files.Count)] `"$($file)`""
    & nuget.exe $arguments
    if (-not $?)  { Write-Host "Error '$file'"; & nuget.exe $arguments; }
  }
}
