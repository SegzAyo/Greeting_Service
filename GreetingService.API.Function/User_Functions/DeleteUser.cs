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
    public class DeleteUser
    {
        private readonly ILogger<DeleteUser> _logger;
        private readonly IUserService _userService;
        private readonly IAuthHandler _authHandler;

        public DeleteUser(ILogger<DeleteUser> log, IUserService userService, IAuthHandler authHandler)
        {
            _logger = log;
            _userService = userService;
            _authHandler = authHandler;
        }

        [FunctionName("DeleteUser")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Accepted, Description = "Accepted")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "user/{email}")] HttpRequest req, string email)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (!await _authHandler.IsAuthorizedAsync(req))
                return new UnauthorizedResult();

            try
            {
                await _userService.DeleteUserAsync(email);
            }
            catch (Exception e)
            {

                throw new Exception(($"There was a conflict: {e.Message}"));
            }

            return new AcceptedResult();
        }
    }
}

