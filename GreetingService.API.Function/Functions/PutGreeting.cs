using System.IO;
using System.Net;
using System.Threading.Tasks;
using GreetingService.API.Function.Authentication;
using GreetingService.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using GreetingService.Core.Entities;
using System;
using GreetingService.Core.Enums;
using GreetingService.Core.Helper_Methods;

namespace GreetingService.API.Function
{
    public class PutGreeting
    {
        private readonly ILogger<PutGreeting> _logger;
        private readonly IGreetingRepository _greetingRepository;
        private readonly IAuthHandler _authHandler;
        private readonly IMessagingService _messageService;


        public PutGreeting(ILogger<PutGreeting> log, IGreetingRepository greetingRepository, IAuthHandler authHandler, IMessagingService messageService)
        {
            _logger = log;
            _greetingRepository = greetingRepository;
            _authHandler = authHandler;
            _messageService = messageService;
        }

        [FunctionName("PutGreeting")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "greeting" })]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "greeting")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (! await _authHandler.IsAuthorizedAsync(req))
                return new UnauthorizedResult();

            //var body = await req.ReadAsStringAsync();
            //var greeting = JsonSerializer.Deserialize<Greeting>(body);

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
                await _messageService.SendAsync(greeting, MessagingServiceSubject.UpdateGreeting);
            }
            catch
            {
                return new ConflictResult();
            }

            return new AcceptedResult();
        }
    }
}

