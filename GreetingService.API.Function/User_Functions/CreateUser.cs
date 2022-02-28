using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using GreetingService.API.Function.Authentication;
using GreetingService.Core;
using GreetingService.Core.Helper_Methods;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace GreetingService.API.Function.User_Functions
{
    public class CreateUser
    {
        private readonly ILogger<CreateUser> _logger;
        private readonly IUserService _userService;
        private readonly IAuthHandler _authHandler;

        public CreateUser(ILogger<CreateUser> log, IUserService userService, IAuthHandler authHandler)
        {
            _logger = log;
            _userService = userService;
            _authHandler = authHandler;
        }

        [FunctionName("CreateUser")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "User" })]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Accepted, Description = "Accepted")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(User), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (!await _authHandler.IsAuthorizedAsync(req))
                return new UnauthorizedResult();

            User user;
            try
            {
                user = System.Text.Json.JsonSerializer.Deserialize<User>(req.Body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }

            if (!EmailAuth.IsValid(user.email))
                throw new FormatException($"wrong email format entered");

            await _userService.CreateUserAsync(user);

            var createdUser = await _userService.GetUserAsync(user.email);

            return new OkObjectResult(createdUser);





        }
    }
}

