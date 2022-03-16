param appName string
param DBPassword string
param SbPassword string
param location string = resourceGroup().location
param DBAdminId string = 'seg-greeting-sql-dev'

param serviceBusNamespaceName string = 'segun-sb-dev${uniqueString(resourceGroup().id)}'
param skuName string = 'Standard'
param SbAccessKeyName string ='RootManageSharedAccessKey'

param kvSkuName string = 'Standard'
param tenantId string = subscription().tenantId
//param SbAccessKeyName string ='RootManageSharedAccessKey'

// storage accounts must be between 3 and 24 characters in length and use numbers and lower-case letters only
var storageAccountName = '${substring(appName,0,10)}${uniqueString(resourceGroup().id)}'
var storageAccountName2 = '${substring(appName,0,10)}${uniqueString(resourceGroup().id)}2' 
var hostingPlanName = '${appName}${uniqueString(resourceGroup().id)}'
var appInsightsName = '${appName}${uniqueString(resourceGroup().id)}'
var appInsightsName2 = '${appName}${uniqueString(resourceGroup().id)}2'
var functionAppName = '${appName}'
var keyVaultNamespaceName = '${appName}keyVault'
var QLserverName = '${appName}${uniqueString(resourceGroup().id)}'
var SQLdatabaseName = '${appName}${uniqueString(resourceGroup().id)}'

resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01' = {
  name: keyVaultNamespaceName
  location: location
  properties: {
    sku: {
      family: 'A'
      name: kvSkuName
    }
    tenantId: tenantId

    enableRbacAuthorization: false      // Using Access Policies model
    accessPolicies: [
      {
        objectId: 'd89101d9-cf97-4b6c-9656-c6da457d8add'
        tenantId: tenantId
        permissions: {
          secrets: [
            'all'
          ]
          certificates: [
            'all'
          ]
          keys: [
            'all'
          ]
        }
      }
    ]

    enabledForDeployment: true          // VMs can retrieve certificates
    enabledForTemplateDeployment: true  // ARM can retrieve values

    enablePurgeProtection: true         // Not allowing to purge key vault or its objects after deletion
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
    createMode: 'default'               // Creating or updating the key vault (not recovering)
  }
}


resource hostingPlan 'Microsoft.Web/serverfarms@2020-10-01' = {
  name: hostingPlanName
  location: location
  sku: {
    name: 'Y1' 
    tier: 'Dynamic'
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02-preview' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: { 
    Application_Type: 'web'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
  tags: {
    // circular dependency means we can't reference functionApp directly  /subscriptions/<subscriptionId>/resourceGroups/<rg-name>/providers/Microsoft.Web/sites/<appName>"
     'hidden-link:/subscriptions/${subscription().id}/resourceGroups/${resourceGroup().name}/providers/Microsoft.Web/sites/${functionAppName}': 'Resource'
  }
}

resource appInsights2 'Microsoft.Insights/components@2020-02-02-preview' = {
  name: appInsightsName2
  location: location
  kind: 'web'
  properties: { 
    Application_Type: 'web'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
  tags: {
    // circular dependency means we can't reference functionApp directly  /subscriptions/<subscriptionId>/resourceGroups/<rg-name>/providers/Microsoft.Web/sites/<appName>"
     'hidden-link:/subscriptions/${subscription().id}/resourceGroups/${resourceGroup().name}/providers/Microsoft.Web/sites/${functionAppName}': 'Resource'
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: storageAccountName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

resource storageAccount2 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: storageAccountName2
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

resource SQLServer 'Microsoft.Sql/servers@2019-06-01-preview' = {
  name: QLserverName
  location: location
  properties: {
    administratorLogin: DBAdminId
    administratorLoginPassword: DBPassword
    }
    resource allowAllWindowsAzureIps 'firewallRules@2021-05-01-preview' = {
      name: 'AllowAllWindowsAzureIps'
      properties: {
        endIpAddress: '0.0.0.0'
        startIpAddress: '0.0.0.0'
      }
    }
  resource SQLdatabase 'databases@2019-06-01-preview' = {
    name: SQLdatabaseName
    location: location
    sku: {
      capacity: 5
      name: 'Basic'
      tier: 'Basic'
    }
  }
}

resource servicebus 'Microsoft.ServiceBus/namespaces@2021-06-01-preview' = {
  name: serviceBusNamespaceName
  location: location
  sku: {
    name: skuName
    tier: 'Standard'
  }
  resource serviceBusTopic 'topics@2021-06-01-preview' = {
    name: 'main'
    properties: {
      defaultMessageTimeToLive: 'P14D'
      status: 'Active'
    }
    resource sbCreateSubscription 'subscriptions@2021-06-01-preview' = {
      name: 'greeting_create'
      properties: {
        deadLetteringOnMessageExpiration: false
        defaultMessageTimeToLive: 'P14D'
        lockDuration: 'PT30S'
        maxDeliveryCount: 10
        status: 'Active'
      }
      resource filterRule 'rules@2021-06-01-preview' = {
        name: 'Subject'
        properties: {
          filterType: 'CorrelationFilter'
          correlationFilter: {
            label: 'NewGreeting'
            properties: {}
          }
        }
      }
    }
    resource sbUpdateGreetingSubscription 'subscriptions@2021-06-01-preview' = {
      name: 'greeting_update'
      properties: {
        deadLetteringOnMessageExpiration: false
        defaultMessageTimeToLive: 'P14D'
        lockDuration: 'PT30S'
        maxDeliveryCount: 10
        status: 'Active'
      }
      resource filterRule 'rules@2021-06-01-preview' = {
        name: 'Subject'
        properties: {
          filterType: 'CorrelationFilter'
          correlationFilter: {
            label: 'UpdateGreeting'
            properties: {}
          }
        }
      }
    }
    resource sbCreateUserSubscription 'subscriptions@2021-06-01-preview' = {
      name: 'user_create'
      properties: {
        deadLetteringOnMessageExpiration: false
        defaultMessageTimeToLive: 'P14D'
        lockDuration: 'PT30S'
        maxDeliveryCount: 10
        status: 'Active'
      }
      resource filterRule 'rules@2021-06-01-preview' = {
        name: 'Subject'
        properties: {
          filterType: 'CorrelationFilter'
          correlationFilter: {
            label: 'NewUser'
            properties: {}
          }
        }
      }
    }
    resource sbUpdateUserSubscription 'subscriptions@2021-06-01-preview' = {
      name: 'user_update'
      properties: {
        deadLetteringOnMessageExpiration: false
        defaultMessageTimeToLive: 'P14D'
        lockDuration: 'PT30S'
        maxDeliveryCount: 10
        status: 'Active'
      }
      resource filterRule 'rules@2021-06-01-preview' = {
        name: 'Subject'
        properties: {
          filterType: 'CorrelationFilter'
          correlationFilter: {
            label: 'UpdateUser'
            properties: {}
          }
        }
      }
    }
    resource sbApprovalUserSubscription 'subscriptions@2021-06-01-preview' = {
      name: 'user_approval'
      properties: {
        deadLetteringOnMessageExpiration: false
        defaultMessageTimeToLive: 'P14D'
        lockDuration: 'PT30S'
        maxDeliveryCount: 10
        status: 'Active'
      }
      resource filterRule 'rules@2021-06-01-preview' = {
        name: 'Subject'
        properties: {
          filterType: 'CorrelationFilter'
          correlationFilter: {
            label: 'NewUser'
            properties: {}
          }
        }
      }
    }
    resource sbGreetingComputeBillingSubscription 'subscriptions@2021-06-01-preview' = {
      name: 'greeting_compute_billing'
      properties: {
        deadLetteringOnMessageExpiration: false
        defaultMessageTimeToLive: 'P14D'
        lockDuration: 'PT30S'
        maxDeliveryCount: 10
        status: 'Active'
      }
      resource filterRule 'rules@2021-06-01-preview' = {
        name: 'Subject'
        properties: {
          filterType: 'CorrelationFilter'
          correlationFilter: {
            label: 'NewGreeting'
            properties: {}
          }
        }
      }
    }
  }
}



resource functionApp 'Microsoft.Web/sites@2020-06-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  properties: {
    httpsOnly: true
    serverFarmId: hostingPlan.id
    clientAffinityEnabled: true
    siteConfig: {
      appSettings: [
        {
          'name': 'APPINSIGHTS_INSTRUMENTATIONKEY'
          'value': appInsights.properties.InstrumentationKey
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
        }
        {
          'name': 'FUNCTIONS_EXTENSION_VERSION'
          'value': '~4'
        }
        {
          name : 'GreetingDbConnectionString'
          value : '@Microsoft.KeyVault(SecretUri=https://${keyVaultNamespaceName}.vault.azure.net/secrets/GreetingDbConnectionString/)'
        }
        {
          name : 'SegBlobConnectionString'
          value : '@Microsoft.KeyVault(SecretUri=https://${keyVaultNamespaceName}.vault.azure.net/secrets/SegBlobConnectionString/)'
        }
        {
          'name': 'FUNCTIONS_WORKER_RUNTIME'
          'value': 'dotnet'
        }
        {
          name : 'WEBSITE_RUN_FROM_PACKAGE'
          value : '1'
        }
        {
          name : 'ServiceBusConnectionString'
          value : '@Microsoft.KeyVault(SecretUri=https://${keyVaultNamespaceName}.vault.azure.net/secrets/ServiceBusConnectionString/)'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
        }
        // WEBSITE_CONTENTSHARE will also be auto-generated - https://docs.microsoft.com/en-us/azure/azure-functions/functions-app-settings#website_contentshare
        // WEBSITE_RUN_FROM_PACKAGE will be set to 1 by func azure functionapp publish
      ]
    }
  }
}

