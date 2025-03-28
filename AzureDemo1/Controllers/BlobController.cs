using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureDemo1.repository;
using Microsoft.AspNetCore.Mvc;

namespace AzureDemo1.Controllers
{
    [ApiController] 
    public class BlobController : ControllerBase
    {
        private readonly BlobServiceClient _blobServiceClient;

        
        public BlobController(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }
        [HttpPost("/api/files/upload")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof((string FileName, string Url)))]
        public async Task<IActionResult> UploadBlob(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            try
            { 
                string containerName = "demo1blobcontainer"; 
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync();

                string uniqueBlobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                BlobClient blobClient = containerClient.GetBlobClient(uniqueBlobName);

                using (Stream stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                return Ok(new { FileName = uniqueBlobName, Url = blobClient.Uri.ToString() });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error uploading to Azure Blob Storage: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error uploading file: {ex.Message}");
            }
        }
        [HttpGet("/api/files/download/{blobName}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(File))]
        public async Task<IActionResult> DownloadBlob(string blobName)
        {
            string containerName = "demo1blobcontainer";
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            if (await containerClient.ExistsAsync())
            {
                BlobClient blobClient = containerClient.GetBlobClient(blobName);
                Stream blobStream = await blobClient.OpenReadAsync();
                Response<BlobProperties> properties = await blobClient.GetPropertiesAsync();
                string contentType = properties.Value.ContentType ?? "application/octet-stream";
                return File(blobStream, contentType, blobName);
                
            }
            else
            {
                Console.Error.WriteLine("Shit where did it go?");
                return NotFound();
            }
        }
    }
}
