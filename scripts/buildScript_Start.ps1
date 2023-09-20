# Start Processes
$processesToStart = @(
    @{ Name = "GO.Main.Services.Exe"; Path = "C:\Users\UserA\Desktop\Vertex\GO - ServerCoordinator\GO.Main.Services.exe" },
    @{ Name = "GO.Main.Services.Exe"; Path = "C:\Users\UserA\Desktop\Vertex\GO - ServerDeviceCenter\GO.Main.Services.exe" }
)

foreach ($processInfo in $processesToStart) {
    $exePath = $processInfo.Path
    if (Test-Path -Path $exePath) {
        $process = Start-Process -FilePath $exePath -PassThru -WindowStyle Minimized
        Write-Host "Started process: $($processInfo.Name), PID: $($process.Id)"
    }
    else {
        Write-Host "Executable file does not exist: $exePath"
        "Executable file does not exist: $exePath"
        exit 1 
    }
}
