$erroractionpreference = "stop"


$suffix = "test"

$resourceGroup = "gruppe2-$suffix"
$hubName = "rg2-iothub"

az account set --subscription "Azure Schulung"

az group create --location "West Europe"   --name $resourceGroup --subscription "Azure Schulung"
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to create resourceGroup"
}


az iot hub create --name $hubName  --resource-group $resourceGroup --subscription "Azure Schulung"
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to create iothub"
}


az iot hub device-identity create --device-id testdevice1 --n $hubName