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
            await getblob.UploadAsync(greetingBinary);

        }

        public async Task DeleteRecordAsync(Guid id)
        {

            var getblobs = _containerClient.GetBlobsAsync();
            var blob_delete = await getblobs.FirstOrDefaultAsync(g => g.Name.Contains(id.ToString()));
            if (blob_delete == null)
                throw new Exception($"Greeting with id: {id} does not exist");
            var blob = await _containerClient.DeleteBlobIfExistsAsync(blob_delete.Name);

            
        }

        public async Task<Greeting> GetAsync(Guid id)
        {
            var getblob = _containerClient.GetBlobsAsync();
            var blob_selected = await getblob.FirstOrDefaultAsync(g => g.Name.Contains(id.ToString()));
            var blobClient = _containerClient.GetBlobClient(blob_selected.Name);
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

        public async Task<IEnumerable<Greeting>> GetAsync(string from, string to)
        {
            var prefix = "";                            //A prefix is literally a prefix on the name, that means it starts from the left. Our blob names are stored like this: {from}/{to}/{id}
            if (!string.IsNullOrWhiteSpace(from))       //only add 'from' to prefix if it's not null
            {
                prefix = from;
                if (!string.IsNullOrWhiteSpace(to))     //only add 'to' to prefix if it's not null and 'from' is not null
                {
                    prefix = $"{prefix}/{to}";          //no wild card support in prefix, only add 'to' to prefix if 'from' also is not null
                }
            }

            var blobs = _containerClient.GetBlobsAsync(prefix: prefix);             //send prefix to the server to only retrieve blobs that matches. The below logic would work even without prefix, but it's slightly optimized if we can send a non empty prefix

            var greetings = new List<Greeting>();
            await foreach (var blob in blobs)                                           //this is how we can asynchronously iterate and process data in an IAsyncEnumerable<T>
            {
                var blobNameParts = blob.Name.Split('/');

                if (!string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to) && blob.Name.StartsWith($"{from}/{to}/"))    //both 'from' and 'to' has values
                {
                    Greeting greeting = await DownloadBlob(blob);
                    greetings.Add(greeting);
                }
                else if (!string.IsNullOrWhiteSpace(from) && string.IsNullOrWhiteSpace(to) && blob.Name.StartsWith($"{from}"))      //'from' has value, 'to' is null
                {
                    Greeting greeting = await DownloadBlob(blob);
                    greetings.Add(greeting);
                }
                else if (string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to) && blobNameParts[1].Equals(to))          //'from' is null, 'to' has value
                {
                    Greeting greeting = await DownloadBlob(blob);
                    greetings.Add(greeting);
                }
                else if (string.IsNullOrWhiteSpace(from) && string.IsNullOrWhiteSpace(to))                                          //both 'from' and 'to' are null
                {
                    Greeting greeting = await DownloadBlob(blob);
                    greetings.Add(greeting);
                }
            }

            return greetings;
        }

        private async Task<Greeting> DownloadBlob(Azure.Storage.Blobs.Models.BlobItem blob)
        {
            var blobClient = _containerClient.GetBlobClient(blob.Name);
            var blobContent = await blobClient.DownloadContentAsync();              //downloading lots of blobs like this will be slow, a more common scenario would be to list metadata for each blob and then download one or more blobs on demand instead of by default downloading all blobs. But we'll roll with this solution in this exercise
            var greeting = blobContent.Value.Content.ToObjectFromJson<Greeting>();
            return greeting;
        }
    }
} 
//
