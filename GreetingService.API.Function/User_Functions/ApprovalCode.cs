using System;
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
using Newtonsoft.Json;

namespace GreetingService.API.Function.User_Functions
{
    public class ApprovalCode
    {
        private readonly ILogger<ApprovalCode> _logger;
        private readonly IUserService _userService;
        private readonly IAuthHandler _authHandler;

        public ApprovalCode(ILogger<ApprovalCode> logger, IUserService userService, IAuthHandler authHandler)
        {
            _logger = logger;
            _userService = userService;
            _authHandler = authHandler;
        }

        [FunctionName("approvalCode")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "User" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(User), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/approve/{code}")] HttpRequest req, string code)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");


            try
            {
                await _userService.ApproveUserAsync(code);
            }
            catch (Exception e)
            {

                throw new Exception($"There was an error: {e.Message}");
            }

            return new AcceptedResult();
        }
    }
}

