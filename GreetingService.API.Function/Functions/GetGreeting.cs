using System;
using System.Collections.Generic;
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
using Newtonsoft.Json;

namespace GreetingService.API.Function
{
    public class GetGreeting
    {
        private readonly ILogger<GetGreeting> _logger;
        private readonly IGreetingRepository _greetingRepository;
        private readonly IAuthHandler _authHandler;

        public GetGreeting(ILogger<GetGreeting> log, IGreetingRepository greetingRepository, IAuthHandler authHandler)
        {
            _logger = log;
            _greetingRepository = greetingRepository;
            _authHandler = authHandler;
        }

        [FunctionName("GetGreeting")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Greeting" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<Greeting>), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "greeting/{id}")] HttpRequest req, string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (! await _authHandler.IsAuthorizedAsync(req))
                return new UnauthorizedResult();

            if (!Guid.TryParse(id, out var Guidid))
                return new BadRequestObjectResult($"{id} is not a valid Guid");

            var greeting = await _greetingRepository.GetAsync(Guidid);

            if (greeting == null)
                return new NotFoundObjectResult("Not found");

            return new OkObjectResult(greeting);
            // Comment
        }
    }
}

