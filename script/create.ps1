$erroractionpreference = "stop"


$suffix = "test"

$resourceGroup = "gruppe2-$suffix"
$hubName = "rg2-iothub"
$datalake = "rg2datalake"
$storageAccount = "rg2group2storage"
$tableValid = "valid"
$tableInvalid = "invalid"

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

# create data lake
Write-Host "datalake"
az dls account create --account $datalake --resource-group $resourceGroup
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to create datalake"
}


# create storage acount
write-host "storage account"
az storage account create --name $storageAccount --resource-group $resourceGroup --sku Standard_LRS
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to create storage account"
}

Write-Host "get keys"
$key = (az storage account keys list -g $resourceGroup -n $storageAccount  --query [0].value -o tsv)
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to get keys"
}

write-host $key

Write-Host "created table $tableValid"

az storage table create --name $tableValid --account-name $storageAccount --account-key $key   
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to create table $tableValid"
}

Write-Host "created table $tableInvalid"
az storage table create --name $tableInvalid --account-name $storageAccount --account-key $key   
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to create table $tableInvalid"
}
