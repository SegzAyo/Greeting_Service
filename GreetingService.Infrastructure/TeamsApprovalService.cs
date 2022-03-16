using GreetingService.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure
{
    public class TeamsApprovalService : IApprovalService
    {
        private readonly HttpClient _httpClient;
        private readonly string _teamsWebHookUrl;      
        private readonly string _greetingServiceBaseUrl;
        private readonly ILogger<TeamsApprovalService> _logger;

        public TeamsApprovalService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<TeamsApprovalService> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _teamsWebHookUrl = configuration["TeamsWebHookUrl"];							//remember to add this to application configuration (local.settings.json, bicep etc)
            _greetingServiceBaseUrl = configuration["GreetingServiceBaseUrl"];              //remember to add this to application configuration (local.settings.json, bicep etc)
            _logger = logger;
        }

        public async Task BeginUserApprovalAsync(User user)
        {
            var Json = @$"{{
                        ""@type"": ""MessageCard"",
                        ""@context"": ""http://schema.org/extensions"",
                        ""themeColor"": ""0076D7"",
                        ""summary"": ""Larry Bryant created a new task"",
                        ""sections"": [{{
                                            ""activityTitle"": ""New subscriber approval request: {user.email}"",
                                            ""activitySubtitle"": ""{user.firstName} {user.lastName}"",
                                            ""activityImage"": ""https://teamsnodesample.azurewebsites.net/static/img/image5.png"",
                                            ""facts"": [{{
                                                            ""name"": ""Subscription Date"",
                                                            ""value"": ""{DateTime.Now:yyyy:MM:dd HH:mm}""
                                                        }}, 
                                                        {{
                                                            ""name"": ""Details"",
                                                            ""value"": ""Please approve or reject the new user: {user.email} for the GreetingService""
                                                        }}
                                                
                                                        ],
                                            ""potentialAction"": [{{
                                                                        ""@type"": ""HttpPOST"",
                                                                        ""name"": ""Approve"",
                                                                        ""target"": ""{_greetingServiceBaseUrl}/api/user/approve/{user.ApprovalCode}""                                                                     
                                                                    }}, 
                                                                    {{
                                                                        ""@type"": ""HttpPOST"",
                                                                        ""name"": ""Reject"",
                                                                        ""target"": ""{_greetingServiceBaseUrl}/api/user/approve/{user.ApprovalCode}""
                                                                                                                                
                                                                 }}]
                                      }}]
                          }}";

            var response = await _httpClient.PostAsync(_teamsWebHookUrl, new StringContent(Json));
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content?.ReadAsStringAsync();
                _logger.LogError("Failed to send approval to Teams for user {email}. Received this response body: {response}", user.email, responseBody?? "null");
            }
            response.EnsureSuccessStatusCode();
        }
    }
}
