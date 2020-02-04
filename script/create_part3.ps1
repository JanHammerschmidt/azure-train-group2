$erroractionpreference = "stop"

$subscriptionName = "Azure Schulung"
$suffix = "test"
$resourceGroup = "rg2-$suffix"
# $location = "WestEurope"

# Names for reused resources
$appServicePlanName = "rg2appserviceplan"

# Names for newly created resources
$webAppName = "rg2WebAppApi"

# Start deploying resources

# Basic set up
az login
az account set --subscription $subscriptionName

az webapp create --resource-group $resourceGroup --name $webAppName --plan $appServicePlanName
if ( $LASTEXITCODE -ne 0 ) {
    THROW "Failed to create web app."
}

