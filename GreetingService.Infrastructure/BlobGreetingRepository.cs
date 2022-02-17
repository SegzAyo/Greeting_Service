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
        //private const string _blobName = "${from}/{to}/{id}.json";
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
            var _blobName = $"{greeting.From}/{greeting.To}/{greeting.Id}.json";
            var getblob = _containerClient.GetBlobClient(_blobName);            
            if (await getblob.ExistsAsync())
                throw new Exception($"Greeting with id: {greeting.Id} already exists");

            var greetingBinary = new BinaryData(greeting, _jsonSerializerOptions);
            await blob.UploadAsync(greetingBinary);

        }

        public async Task DeleteRecordAsync(Guid id)
        {

            var getblobs = _containerClient.GetBlobsAsync();
            var blob_delete = await getblobs.FirstOrDefaultAsync(g => g.Name.EndsWith(id.ToString()));
            if (blob_delete == null)
                throw new Exception($"Greeting with id: {id} does not exist");
            var blob = await _containerClient.DeleteBlobIfExistsAsync(blob_delete.Name);

            
        }

        public async Task<Greeting> GetAsync(Guid id)
        {
            var getblob = _containerClient.GetBlobClient();
            if (!await getblob.ExistsAsync())
                throw new Exception($"Greeting with id: {id} not found");

            var blobContent = await getblob.DownloadContentAsync();
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
            var _blobName = $"{greeting.From}/{greeting.To}/{greeting.Id}.json";
            var blobClient = _containerClient.GetBlobClient(greeting.Id.ToString());
            await blobClient.DeleteIfExistsAsync();
            var greetingBinary = new BinaryData(greeting, _jsonSerializerOptions);
            await blobClient.UploadAsync(greetingBinary);
        }
    }
} 
//
