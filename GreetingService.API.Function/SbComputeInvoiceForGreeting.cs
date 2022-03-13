using System;
using System.Linq;
using System.Threading.Tasks;
using GreetingService.Core;
using GreetingService.Core.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace GreetingService.API.Function
{
    public class SbComputeInvoiceForGreeting
    {
        private readonly ILogger<SbComputeInvoiceForGreeting> _logger;
        private readonly IInvoiceService _invoiceService;
        private readonly IGreetingRepository _greetingRepository;
        private readonly IUserService _userService;
        public SbComputeInvoiceForGreeting(ILogger<SbComputeInvoiceForGreeting> log, IInvoiceService invoiceService, IGreetingRepository greetingRepository)
        {
            _logger = log;
            _invoiceService = invoiceService;
        }

        [FunctionName("SbComputeInvoiceForGreeting")]
        public async Task Run([ServiceBusTrigger("main", "greeting_compute_invoice", Connection = "ServiceBusConnectionString")]Greeting greeting)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {greeting}");

            try
            {
                var invoice = await _invoiceService.GetInvoiceAsync(greeting.Timestamp.Year, greeting.Timestamp.Month, greeting.From);          //This method returns null if invoice not found
                var user = await _userService.GetUserAsync(greeting.From);

                if (invoice == null)                                                        //Invoice does not exist, create a new invoice
                {
                    try
                    {
                        invoice = new Invoice
                        {
                            Month = greeting.Timestamp.Month,
                            Year = greeting.Timestamp.Year,
                            User = user,
                        };
                        await _invoiceService.CreateOrUpdateInvoiceAsync(invoice);

                        invoice = await _invoiceService.GetInvoiceAsync(greeting.Timestamp.Year, greeting.Timestamp.Month, greeting.From);
                        invoice.SentGreetings = invoice.SentGreetings.Append(greeting).ToList();
                        await _invoiceService.CreateOrUpdateInvoiceAsync(invoice);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Failed to create new invoice for Greeting {id}", greeting.Id);
                        throw;
                    }
                }
                else if (!invoice.SentGreetings.Any(x => x.Id == greeting.Id))                  //Invoice is not null (it exists) and it does not already contain this Greeting, update existing invoice
                {
                    try
                    {
                        invoice.SentGreetings = invoice.SentGreetings.Append(greeting).ToList();
                        await _invoiceService.CreateOrUpdateInvoiceAsync(invoice);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Failed to update invoice {id} with new Greeting {greetingId}", invoice.Id, greeting.Id);
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed compute invoice for new greeting {id}", greeting.Id);
                throw;
            }
        }
    }
}
