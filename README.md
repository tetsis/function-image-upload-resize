# function-image-upload-resize

## Architecture
```
           |-Blob Storage-------|            |-Event Grid-|
- Upload ->| "images" container |- Created ->| topic      |--
           |--------------------|            |------------| |
                                                            |
                                                  Subscribe |
                                                            |
|-Blob Storage-----------|           |-Azure Functions-|    |
| "thumbnails" container |<- Upload -| funciton app    |<----
|------------------------|           |-----------------|
```

## Prerequisites
- Azure subscription
- Resource group
- Storage account
    - Container for original image (e.g. "images")
    - Container for thumbnail (e.g. "thumbnails")
    - These two containers can be the same or different storage accounts.
- Function app

## How to deploy
Execute the following commands using Azure CLI or [Cloud Shell](https://learn.microsoft.com/en-us/azure/cloud-shell/overview).

- Specify the name for resources.
```
resourceGroupName="myResourceGroup"
blobStorageAccount="myStorageAccountForThumbnails"
functionapp="myFunctionapp"
```

- Configure the funciton app
```
storageConnectionString=$(az storage account show-connection-string --resource-group $resourceGroupName --name $blobStorageAccount --query connectionString --output tsv)
az functionapp config appsettings set --name $functionapp --resource-group $resourceGroupName --settings AzureWebJobsStorage=$storageConnectionString THUMBNAIL_CONTAINER_NAME=thumbnails THUMBNAIL_WIDTH=100
```

- Deploy the function code
```
az functionapp deployment source config --name $functionapp --resource-group $resourceGroupName --branch main --manual-integration --repo-url https://github.com/tetsis/function-image-upload-resize
```

- Create an event subscription
	- https://learn.microsoft.com/en-us/azure/event-grid/resize-images-on-storage-blob-upload-event#create-an-event-subscription

## References
- [Tutorial: Use Azure Event Grid to automate resizing uploaded images - Azure Event Grid | Microsoft Learn](https://learn.microsoft.com/en-us/azure/event-grid/resize-images-on-storage-blob-upload-event)
- [Azure-Samples/function-image-upload-resize: Sample function in Azure Functions that demonstrates how to upload and resize images in Azure Storage.](https://github.com/Azure-Samples/function-image-upload-resize)
