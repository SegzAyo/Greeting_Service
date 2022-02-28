using System.IO;
using System.Net;
using System.Threading.Tasks;
using GreetingService.API.Function.Authentication;
using GreetingService.Core;
using GreetingService.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Net.Mail;
using System;
using GreetingService.Core.Helper_Methods;

namespace GreetingService.API.Function
{
    public class PostGreeting
    {
        private readonly ILogger<GetGreetings> _logger;
        private readonly IGreetingRepository _greetingRepository;
        private readonly IAuthHandler _authHandler;

        public PostGreeting(ILogger<GetGreetings> log, IGreetingRepository greetingRepository, IAuthHandler authHandler)
        {
            _logger = log;
            _greetingRepository = greetingRepository;
            _authHandler = authHandler;
        }

        [FunctionName("PostGreeting")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Greeting" })]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Accepted, Description = "Accepted")]
        
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "greeting")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if(! await _authHandler.IsAuthorizedAsync(req))
                return new UnauthorizedResult();

            var body = await req.ReadAsStringAsync();
            var greeting = JsonSerializer.Deserialize<Greeting>(body);

            if (!EmailAuth.IsValid(greeting.From))
                throw new FormatException($"wrong format entered: {body}");

            if (!EmailAuth.IsValid(greeting.To))
                throw new FormatException($"wrong format entered: {body}");

            try
            {
                await _greetingRepository.CreateAsync(greeting);
            }
            catch
            {

                return new ConflictResult();
            }
            
            return new AcceptedResult();
        }
    }
}

