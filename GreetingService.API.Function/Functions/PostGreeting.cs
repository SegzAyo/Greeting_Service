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
using Azure.Messaging.ServiceBus;
using GreetingService.Core.Enums;




namespace GreetingService.API.Function
{
    public class PostGreeting
    {
        private readonly ILogger<GetGreetings> _logger;
        private readonly IGreetingRepository _greetingRepository;
        private readonly IAuthHandler _authHandler;
        private readonly IMessagingService _messageService;

        public PostGreeting(ILogger<GetGreetings> log, IGreetingRepository greetingRepository, IAuthHandler authHandler, IMessagingService messageService)
        {
            _logger = log;
            _greetingRepository = greetingRepository;
            _authHandler = authHandler;
            _messageService = messageService;
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

            //var body = await req.ReadAsStringAsync();
            //var greeting = JsonSerializer.Deserialize<Greeting>(body);

            //if (!EmailAuth.IsValid(greeting.From))
            //    throw new FormatException($"wrong format entered: {body}");

            //if (!EmailAuth.IsValid(greeting.To))
            //    throw new FormatException($"wrong format entered: {body}");

            Greeting greeting;

            try
            {
                var body = await req.ReadAsStringAsync();
                greeting = JsonSerializer.Deserialize<Greeting>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
            try
            {
                if (!EmailAuth.IsValid(greeting.From))
                    return new BadRequestObjectResult($"wrong email format entered");
                if (!EmailAuth.IsValid(greeting.To))
                    return new BadRequestObjectResult($"wrong email format entered");

                await _messageService.SendAsync(greeting, MessagingServiceSubject.NewGreeting);
            }
            catch
            {
                return new ConflictResult();
            }

            return new AcceptedResult();
        }
    }
}

