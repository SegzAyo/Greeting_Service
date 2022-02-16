using Azure.Storage.Blobs;
using GreetingService.Core.Entities;
using GreetingService.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure
{
    public class BlobGreetingRepository : IGreetingRepository
    {
        private readonly string _connectionString;
        private const string _containerName = "blobgreeting";
        private const string _blobName = "greetings.json";
        private readonly BlobContainerClient _containerClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

        public BlobGreetingRepository(IConfiguration configuration)
        {
            _connectionString = configuration["SegBlobConnectionString"];

            _containerClient = new BlobContainerClient(_connectionString, _containerName);
            _containerClient.CreateIfNotExists();
        }

        public async Task CreateAsync(Greeting greeting)
        {
            var blob = _containerClient.GetBlobClient(greeting.Id.ToString());            
            if (await blob.ExistsAsync())
                throw new Exception($"Greeting with id: {greeting.Id} already exists");

            var greetingBinary = new BinaryData(greeting, _jsonSerializerOptions);
            await blob.UploadAsync(greetingBinary);

        }

        public async Task DeleteRecordAsync(Guid id)
        {
            var blob = await _containerClient.DeleteBlobIfExistsAsync(id.ToString());

            throw new Exception($"Greeting with id: {id} does not exist");
        }

        public async Task<Greeting> GetAsync(Guid id)
        {
            var blobClient = _containerClient.GetBlobClient(id.ToString());
            if (!await blobClient.ExistsAsync())
                throw new Exception($"Greeting with id: {id} not found");

            var blobContent = await blobClient.DownloadContentAsync();
            var greeting = blobContent.Value.Content.ToObjectFromJson<Greeting>();
            return greeting;
        }

        public async Task<IEnumerable<Greeting>> GetAsync()
        {
            var greetings = new List<Greeting>();
            var blobs = _containerClient.GetBlobsAsync();                           
            await foreach (var blob in blobs)                                           
            {
                var blobClient = _containerClient.GetBlobClient(blob.Name);
                var blobContent = await blobClient.DownloadContentAsync();              
                var greeting = blobContent.Value.Content.ToObjectFromJson<Greeting>();
                greetings.Add(greeting);
            }

            return greetings;
        }

        public async Task UpdateAsync(Greeting greeting)
        {
            var blobClient = _containerClient.GetBlobClient(greeting.Id.ToString());
            await blobClient.DeleteIfExistsAsync();
            var greetingBinary = new BinaryData(greeting, _jsonSerializerOptions);
            await blobClient.UploadAsync(greetingBinary);
        }
    }
}
