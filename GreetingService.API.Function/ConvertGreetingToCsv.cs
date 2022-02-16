using System;
using System.IO;
using System.Threading.Tasks;
using GreetingService.API.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace GreetingService.API.Function
{
    public class ConvertGreetingToCsv
    {
        [FunctionName("ConvertGreetingToCsv")]
        public async Task Run([BlobTrigger("samples-workitems/{name}", Connection = "SegBlobConnectionString")] Stream greetingJsonBlob, string name, [Blob("greetings-csv/{name}", FileAccess.Write, Connection = "LoggingStorageAccount")] Stream greetingCsvBlob, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {greetingJsonBlob.Length} Bytes");

            var streamWriter = new StreamWriter(greetingCsvBlob);       
            streamWriter.WriteLine("id;from;to;message;timestamp");     
            streamWriter.WriteLine($"{Greeting.Id};{greeting.from};{greeting.To};{greeting.Message};{greeting.Timestamp}");  
            await streamWriter.FlushAsync();
        }
    }
}
