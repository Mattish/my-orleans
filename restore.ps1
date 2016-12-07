$files = Get-ChildItem -Path "." -File -Recurse -Filter "*.csproj"
for ($i = 0; $i -lt $files.Length; $i++) {
    Write-Host -NoNewline ("Restoring " + $files[$i].Name + "...")
    $args = ("restore " + $files[$i].FullName);
    $process = Start-Process -FilePath "dotnet.exe" -Args $args -Wait -WindowStyle Hidden -PassThru
    
    if($process.ExitCode.Equals(0)){
        Write-Host -ForegroundColor Green "Success"
    }
    else{
        Write-Host -ForegroundColor Red "Fail"
    }
}
Write-Host "Done." 