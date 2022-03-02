using System.IO;
using System.Net;
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

namespace GreetingService.API.Function
{
    public class GetInvoice
    {
        private readonly ILogger<GetInvoice> _logger;
        private readonly IAuthHandler _authHandler;
        private readonly IInvoiceService _invoiceService;

        public GetInvoice(ILogger<GetInvoice> log, IAuthHandler authHandler, IInvoiceService invoiceService)
        {
            _logger = log;
            _authHandler = authHandler;
            _invoiceService = invoiceService;
        }

        [FunctionName("GetInvoice")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Invoice" })]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Accepted, Description = "Accepted")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "invoice/{year}/{month}/{email}")] HttpRequest req, int year, int month, string email)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (!await _authHandler.IsAuthorizedAsync(req))
                return new UnauthorizedResult();

            if (!EmailAuth.IsValid(email))
                return new BadRequestObjectResult($"{email} is not a valid email address");

            var invoices = await _invoiceService.GetInvoiceAsync(year, month, email);
            return new OkObjectResult(invoices);
        }
    }
}

