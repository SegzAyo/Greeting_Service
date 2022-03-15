using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using GreetingService.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace GreetingService.API.Function.SB_Function
{
    public class sbBeginUserApproval
    {
        private readonly ILogger<sbBeginUserApproval> _logger;
        private readonly IApprovalService _approvalService;

        public sbBeginUserApproval(ILogger<sbBeginUserApproval> log, IApprovalService approvalService)
        {
            _logger = log;
            _approvalService = approvalService;
        }

        [FunctionName("SbBeginUserApproval")]
        public async Task Run([ServiceBusTrigger("main", "user_approval", Connection = "ServiceBusConnectionString")] User user)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {user}");

            try
            {
                await _approvalService.BeginUserApprovalAsync(user);
            }
            catch (Exception e)
            {
                _logger.LogError("Failed begin user approval", e);
                throw;
            }
        }
    }
}

