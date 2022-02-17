using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GreetingService.Core.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;


namespace GreetingService.API.Function
{
    public class ConvertGreetingToCsv
    {
        [FunctionName("ConvertGreetingToCsv")]
        public async Task Run([BlobTrigger("blobgreeting/{name}", Connection = "SegBlobConnectionString")] Stream greetingJsonBlob, string name, [Blob("greetings-csv/{name}", FileAccess.Write, Connection = "SegBlobConnectionString")] Stream greetingCsvBlob, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {greetingJsonBlob.Length} Bytes");

            var greeting = JsonSerializer.Deserialize<Greeting>(greetingJsonBlob);
            var streamWriter = new StreamWriter(greetingCsvBlob);
            streamWriter.WriteLine("id;from;to;message;timestamp");
            streamWriter.WriteLine($"{greeting.Id};{greeting.From};{greeting.To};{greeting.Message};{greeting.Timestamp}");
            await streamWriter.FlushAsync();
        }
    }
}
