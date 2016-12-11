Import-Module Posh-SSH
Import-Module ($PSScriptRoot + "\..\DotnetcoreRemoteInstall\DotnetcoreRemoteInstall.psm1")

function Install-Project
{
    Param (
        [Parameter(Mandatory=$True)]
        [string]$RemoteHost,
        [Parameter(Mandatory=$True)] 
        [string]$Username,
        [Parameter(Mandatory=$True)] 
        [string]$KeyFile,
        [Parameter(Mandatory=$True)]
        [string]$ProjectName
    )

    Add-Type -AssemblyName "System.IO.Compression.FileSystem"

    $sshSession = New-LocalSSHSession -RemoteHost $RemoteHost -Username $Username -KeyFile $KeyFile 
    $sftpSession = New-LocalSFTPSession -RemoteHost $RemoteHost -Username $Username -KeyFile $KeyFile
    Write-Host ("Deploying " + $ProjectName + " to " + $RemoteHost + "...") 
    if((Test-Dotnetcore -SSHSession $sshSession) -eq $false){
        Write-Host -ForegroundColor Green "Failed."
        Write-Host "Remote system does not have dotnetcore installed."
        Start-Sleep -Seconds 1
        return
    }
    Write-Host -ForegroundColor Green "Done."

    $localFolder = ((Get-Location).Path + "\"+ $ProjectName + "\bin\Debug\netcoreapp1.0\publish")
    $remotePath = ("./deployables/" + $ProjectName + "/")

    $localZip = (Get-Location).Path + "\" + $ProjectName + "_deployable.zip"
    if(Test-Path $localZip){
        Remove-Item $localZip
    }
    [IO.Compression.ZipFile]::CreateFromDirectory($localFolder, $localZip)

    $files = Get-ChildItem -Path $localFolder

    try {
        Set-SFTPLocation -SFTPSession $sftpSession -Path $remotePath
    }
    catch {
        try{
            $createFolderTask = $(New-SFTPItem -SFTPSession $sftpSession -Path $remotePath -Recurse -ItemType "Directory")
            Set-SFTPLocation -SFTPSession $sftpSession -Path $remotePath
        }
        catch{
            Write-Host $_.Exception.Message
            Write-Host -ForegroundColor Red "Unable create host deployable folder, exiting."
            $sftpSession.Disconnect();
            $sshSession.Disconnect();
            Exit-PSSession
        }
    }
    finally{
        Write-Host ("Copying " + $localZip + " to " + $remotePath + "...") -NoNewline
        Set-SFTPFile -SFTPSession $sftpSession -LocalFile $localZip -RemotePath "./" -Overwrite
        Write-Host -ForegroundColor Green ("Done.")
        Write-Host ("Extracting...") -NoNewline
        $folderRmTask = $(Invoke-SSHCommand -SSHSession $sshSession -Command ("apt-get --assume-yes install unzip") -TimeOut 10000)
        # $folderRmTask = $(Invoke-SSHCommand -SSHSession $sshSession -Command ("rm -r ~/"+ $remotePath + "*") -TimeOut 10000)
        $unzipTask = $(Invoke-SSHCommand -SSHSession $sshSession -Command ("unzip -o -d ~/" + $remotePath + " ~/" + $remotePath + $ProjectName + "_deployable.zip") -TimeOut 10000)
        Write-Host -ForegroundColor Green ("Done.")
        $sshSession.Disconnect();
    }

    $sftpSession.Disconnect();
    Write-Host -ForegroundColor Green "Finish."
    Start-Sleep -Seconds 1
}

$remoteHost = "81fbce23-4d2e-411c-87a9-0c5f6e246292.pub.cloud.scaleway.com"
$keyfile = ".\openssh_private.rsa"

if((Test-Dotnetcore -RemoteHost $remoteHost -Username "root" -KeyFile $keyfile) -eq $false){
    Install-Dotnetcore -RemoteHost $remoteHost -Username "root" -KeyFile $keyfile
}

Install-Project -RemoteHost $remoteHost -Username "root" -KeyFile $keyfile -ProjectName "Client"