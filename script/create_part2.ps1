$erroractionpreference = "stop"

$subscriptionName = "Azure Schulung"
$suffix = "test"
$resourceGroup = "rg2-$suffix"
$location = "WestEurope"

# Storage account name. Should be the same as in create.ps1
$storageAccountName = "rg2storageaccount"


# Names for newly created resources
$appServicePlanName = "rg2appserviceplan"
$functionAppName = "rg2functionapp-dailyreport"
$insightsName = "rg2applicationinsights"

# Start deploying resources

# Basic set up
az login
az account set --subscription $subscriptionName
az extension add --name application-insights

az monitor app-insights component create --location $location --resource-group $resourceGroup --app $insightsName --kind web
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to create app insights."
}


# We must create the appservice plan in tier B1 b/c the function app initially requires
# the alwayson feature which is not available in the free plan.
# However, after disabling alwayson for the functionapp, we can downgrade to F1.

az appservice plan create --location $location --resource-group $resourceGroup --name $appServicePlanName --sku B1
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to create app service plan."
}

az functionapp create --resource-group $resourceGroup --name $functionAppName --storage-account $storageAccountName --app-insights $insightsName --plan $appServicePlanName --runtime dotnet
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to create function app."
}


az functionapp config set --always-on false
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to set function app to always-on:false."
}
az appservice plan update --sku F1
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to set app service plan to free tier."
}

