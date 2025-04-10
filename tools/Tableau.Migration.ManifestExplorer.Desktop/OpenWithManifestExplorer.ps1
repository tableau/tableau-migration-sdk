# Check if the script is running with administrator privileges
if (-not ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    # Re-launch the script with elevated privileges
    $newProcess = Start-Process powershell -ArgumentList "-File `"$PSCommandPath`"" -Verb RunAs -PassThru
    $newProcess.WaitForExit()
    exit
}

try {
    # Get the directory of the current script
    $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition

    # Define the path to your program executable
    $programPath = Join-Path $scriptDir "Tableau.Migration.ManifestExplorer.Desktop.exe"

    # Define the registry key path for .json files
    $regPath = "HKLM:\Software\Classes\SystemFileAssociations\.json\shell\OpenWithManifestExplorer"

    # Create the registry key for the context menu item and set the default value
    New-Item -Path $regPath -Force -Value "Open with Manifest Explorer" -ErrorAction Stop | Out-Null

    # Create the command subkey and set the command to run your program with the selected file as an argument
    New-Item -Path "$regPath\command" -Force -Value "`"$programPath`" `"%1`"" -ErrorAction Stop | Out-Null

    # Define the path to the icon file
    $iconPath = Join-Path $scriptDir "ManifestExplorerIcon.ico"

    # Set the icon for the context menu item
    Set-ItemProperty -Path $regPath -Name "Icon" -Value $iconPath -ErrorAction Stop

    Write-Output "Context menu item added successfully."    
}
catch {
    # Write the error details
    Write-Error "Context menu failed to be added. Error: $_"
    Write-Error "Detailed Error Message: $($_.Exception.Message)"
    Write-Error "Stack Trace: $($_.Exception.StackTrace)"
}
