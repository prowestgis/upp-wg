# Command parameters
param (
    [Parameter(Mandatory=$true)][string]$Computer,
    [Parameter(Mandatory=$true)][string]$RemoteDir,
    [Parameter(Mandatory=$true)][string]$RemoteNssmPath
)
    
# List of services that need to be spun up
$ServiceData =
    ( "AxleInformation", "UPP Axle Information", $null, 56491 ),
    ( "CompanyInformation", "UPP Company Information", $null, 56486 ),
    ( "InsuranceInformation", "UPP Insurance Information", $null, 56487 ),
    ( "Manager", "UPP Manager", $null, 56484 ),
    ( "PermitIssuer", "UPP Permit Authority - Beltrami", "Beltrami", 56500 ),
    ( "PermitIssuer", "UPP Permit Authority - Cass", "Cass", 56501 ),
    ( "PermitIssuer", "UPP Permit Authority - Clearwater", "Clearwater", 56502 ),
    ( "PermitIssuer", "UPP Permit Authority - Duluth", "Duluth", 56507 ),
    ( "PermitIssuer", "UPP Permit Authority - Hubbard", "Hubbard", 56503 ),
    ( "PermitIssuer", "UPP Permit Authority - Itasca", "Itasca", 56504 ),
    ( "PermitIssuer", "UPP Permit Authority - Polk", "Polk", 56505 ),
    ( "PermitIssuer", "UPP Permit Authority - St. Louis", "StLouis", 56506 ),
    ( "ServiceDirectory", "UPP Service Directory", $null, 56485 ),
    ( "TrailerInformation", "UPP Trailer Information", $null, 56489 ),
    ( "TruckInformation", "UPP Truck Information", $null, 56490 ),
    ( "VehicleInformation", "UPP Vehicle Information", $null, 56488 )`
    | ForEach-Object {[pscustomobject]@{Name = $_[0]; ServiceName = $_[1]; Identifier = $_[2]; LocalPort = $_[3]}}

# $ServiceData | Format-Table Name, ServiceName, LocalPort -Auto

# Notify the user where the services are getting published
"Deploying microservices to {0}" -f $Computer | Write-Output 

# Look for all of the UPP services on the remote machine that match the list
$ServiceNames = $ServiceData | Select-Object -ExpandProperty ServiceName
$current_services = Get-Service -ComputerName $Computer | Where-Object { $ServiceNames -contains $_.name }

# Filter out the services that are currently running
$running_services = $current_services | Where-Object { $_.Status -eq "Running" }

"Running services"
$running_services | Write-Output

# Stop the existing services
# $running_services | Stop-Service

# For each of the services, copy the built project artifacts,
# update the application configuration file and then
# run NSSM to register the service
#$session = New-PSSession -ComputerName $Computer
$root = Join-Path -Path (Get-Location) -ChildPath ".." -Resolve
$ServiceData | ForEach-Object -Process {
    $bin = Join-Path -Path $root -ChildPath "$($_.Name)/bin/Debug" -Resolve
    $dest = Join-Path -Path $RemoteDir -ChildPath $_.Name

    "$($bin) -> $($dest)" | Write-Output
    # Copy-Item -Path $bin -Destination $dest -ToSession $session -Recurse

    $exe = Join-Path -Path $dest -ChildPath "$($_.Name).exe"
    $cmd = "$($RemoteNssmPath) install `"$($_.ServiceName)`" `"$($exe)`""
    $cmd | Write-Output
    # $Invoke-Command 

}
#$session | Remove-PSSession

# Verify that all of the services are up and running...
