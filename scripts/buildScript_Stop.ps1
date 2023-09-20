# Stop Processes
$processPathsToStop = @(
    "C:\Users\UserA\Desktop\Vertex\GO - ServerDeviceCenter\GO.Main.Services.exe",
    "C:\Users\UserA\Desktop\Vertex\GO - ServerCoordinator\GO.Main.Services.exe"
)

foreach ($processPath in $processPathsToStop) {
    if (Test-Path -Path $processPath) {
        $processes = Get-Process | Where-Object { $_.Path -eq $processPath }

        if ($processes.Count -eq 0) {
            Write-Host "No process found for executable: $processPath"
        }
        else {
            foreach ($process in $processes) {
                $process | Stop-Process -Force
                Write-Host "Stopped process: $($process.ProcessName), PID: $($process.Id)"
            }
        }
    }
    else {
        Write-Host "Executable file does not exist: $processPath"
        "Executable file does not exist: $processPath"
        exit 1
    }
}
