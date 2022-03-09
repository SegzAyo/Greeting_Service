using System;
using System.Threading.Tasks;
using GreetingService.Core;
using GreetingService.Core.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace GreetingService.API.Function.SB_Function
{
    public class SbUpdateGreeting
    {
        private readonly ILogger<SbUpdateGreeting> _logger;
        private readonly IGreetingRepository _greetingRepository;

        public SbUpdateGreeting(ILogger<SbUpdateGreeting> log, IGreetingRepository greetingRepository)
        {
            _logger = log;
            _greetingRepository = greetingRepository;
        }

        [FunctionName("SbUpdateGreeting")]
        public async Task Run([ServiceBusTrigger("main", "greeting_update", Connection = "ServiceBusConnectionString")]Greeting greeting)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {greeting}");

            try
            {
                await _greetingRepository.UpdateAsync(greeting);
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to update Greeting to IGreetingRepository", e);
                throw;
            }
        }
    }
}
