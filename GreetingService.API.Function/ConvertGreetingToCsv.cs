using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace GreetingService.API.Function
{
    public class ConvertGreetingToCsv
    {
        [FunctionName("ConvertGreetingToCsv")]
        public void Run([BlobTrigger("samples-workitems/{name}", Connection = "XU3R7K3NLk1TUabiQb9ky2SBbOMsWOLhcrt+hzh8h8yCrxZxBiEiDqs5VFPD6cYn5h7rGRbJBICNpYHMsZvK1A==")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
