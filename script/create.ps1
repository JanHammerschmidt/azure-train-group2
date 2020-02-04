Import-Module Az
$erroractionpreference = "stop"

$subscriptionName = "Azure Schulung"
$suffix = "test"
$resourceGroup = "rg2-$suffix"
$location = "WestEurope"

$lockName = "NoDeleteLock"

$hubName = "rg2iothub"
$testDeviceName = "testdevice1"

$streamAnalyticsName = "rg2streamanalytics"
$storageAccountName = "rg2storageaccount"

# Storage account stuff
$dataLakeName = "sensordata"
$validDataTableName = "sensorvalid"
$errorDataTableName = "sensorerror"


# Start deploying resources

# Basic set up
az login
az account set --subscription $subscriptionName
az extension add --name azure-cli-iot-ext

# Basic set up for Powershell
Connect-AzAccount
Get-AzSubscription -SubscriptionName $subscriptionName | Select-AzSubscription


# Resource group
az group create --location $location --name $resourceGroup 
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to create resourceGroup"
}

# Hub + Test device
az iot hub create --location $location --name $hubName  --resource-group $resourceGroup --sku B1
 
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to create iothub"
}
az iot hub device-identity create --device-id $testDeviceName --hub-name $hubName 
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to create test device on iothub"
}

# Stream analytics job
New-AzStreamAnalyticsJob -ResourceGroupName $resourceGroup -File "streamanalyticsjobconfig.json" -Name $streamAnalyticsName -Force
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to create stream analytics job"
}

# Storage account and its resources

az storage account create --name $storageAccountName --resource-group $resourceGroup --sku Standard_LRS --kind StorageV2 --enable-hierarchical-namespace true --location $location
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to create storage account"
}

az storage container create --account-name $storageAccountName --name $dataLakeName
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to create gen2 data lake"
}

az storage table create --name $validDataTableName --account-name $storageAccountName 
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to create $validDataTableName"
}

az storage table create --name $errorDataTableName --account-name $storageAccountName 
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to create $errorDataTableName"
}

az lock create --lock-type CanNotDelete --name $lockName --resource-group $resourceGroup
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to create resource lock. This can happen if you have contributor rights only. As this is the last step of the script, it's not too bad and all the resources have been created."
}

