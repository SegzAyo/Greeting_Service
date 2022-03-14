using System;
using System.Threading.Tasks;
using GreetingService.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace GreetingService.API.Function.SB_Function
{
    public class SbCreateUser
    {
        private readonly ILogger<SbCreateUser> _logger;
        private readonly IUserService _userService;


        public SbCreateUser(ILogger<SbCreateUser> log, IUserService userService)
        {
            _logger = log;
            _userService = userService;

        }

        [FunctionName("SbCreateUser")]
        public async Task Run([ServiceBusTrigger("main", "user_create", Connection = "ServiceBusConnectionString")]User user)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {user}");

            try
            {
                await _userService.CreateUserAsync(user);
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to insert Greeting to IGreetingRepository", e);
                throw;
            }
        }
    }
}
