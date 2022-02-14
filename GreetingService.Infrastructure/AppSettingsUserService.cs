using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreetingService.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GreetingService.Infrastructure
{
    public class AppSettingsUserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AppSettingsUserService> _logger;

        public AppSettingsUserService(IConfiguration configuration, ILogger<AppSettingsUserService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public bool IsValidUser(string username, string password)
        {
            var entries = _configuration.AsEnumerable().ToDictionary(x => x.Key, x => x.Value);
            if (entries.TryGetValue(username, out var storedPassword))
            {
                if (storedPassword == password)
                {
                    _logger.LogInformation("Valid credentials for {username}", username);
                    return true;
                }
            }

            _logger.LogWarning("Invalid credentials for {username}", username);
            return false;

        }
    }
};
