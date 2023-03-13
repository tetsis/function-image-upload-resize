// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;
using System.Text.Json;
using Azure.Messaging.EventGrid.SystemEvents;
using Azure.Storage.Blobs;
using System.Threading.Tasks;

namespace function_image_upload_resize
{
    public static class DeleteThumbnail
    {
        private static readonly string BLOB_STORAGE_CONNECTION_STRING = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        private static string GetBlobNameFromUrl(string bloblUrl)
        {
            var uri = new Uri(bloblUrl);
            var blobClient = new BlobClient(uri);
            return blobClient.Name;
        }

        [FunctionName("DeleteThumbnail")]
        public static async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            var deletedEvent = JsonSerializer.Deserialize<StorageBlobDeletedEventData>(eventGridEvent.Data.ToString());
            var blobName = GetBlobNameFromUrl(deletedEvent.Url);

            var thumbContainerName = Environment.GetEnvironmentVariable("THUMBNAIL_CONTAINER_NAME");
            var blobServiceClient = new BlobServiceClient(BLOB_STORAGE_CONNECTION_STRING);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(thumbContainerName);

            await blobContainerClient.DeleteBlobAsync(blobName);
        }
    }
}
