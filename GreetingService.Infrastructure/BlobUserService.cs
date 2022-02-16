using Azure.Storage.Blobs;
using GreetingService.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure
{
    public class BlobUserService : IUserService
    {
        private readonly string _connectionString;
        private readonly BlobContainerClient _containerClient;
        private const string _containername = "users";
        private const string _blobname = "user.json";
        private readonly ILogger<BlobUserService> logger;

        public BlobUserService(IConfiguration configuration)
        {
            _connectionString = configuration["SegBlobConnectionString"];
            _containerClient = new BlobContainerClient(_connectionString, _containername);
            _containerClient.CreateIfNotExists();
        }

        public bool IsValidUser(string username, string password)
        {
            var blob = _containerClient.GetBlobClient(_blobname);

            if (!blob.Exists())
                return false;

            var blobContent = blob.DownloadContent();
            var usersDictionary = blobContent.Value.Content.ToObjectFromJson<IDictionary<string, string>>();

            if (usersDictionary.TryGetValue(username, out var storedPassword))
            {
                if (storedPassword.Equals(password))
                    return true;
            }

            return false;
        }
    }
}