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
    public class TimerInvoiceCalc
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IGreetingRepository _greetingRepository;
        private readonly IUserService _userService;

        public TimerInvoiceCalc(IInvoiceService invoiceService, IGreetingRepository greetingRepository, IUserService userService)
        {
            _invoiceService = invoiceService;
            _greetingRepository = greetingRepository;
            _userService = userService;
        }

        [FunctionName("TimerInvoiceCalc")]
        public async Task Run([TimerTrigger("0 0 0 */1 * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var greetings = await _greetingRepository.GetAsync();

            var greetingsGroupedByInvoice = greetings.GroupBy(x => new { x.From, x.Timestamp.Year, x.Timestamp.Month });

            foreach (var group in greetingsGroupedByInvoice)
            {
                var user = await _userService.GetUserAsync(group.Key.From);     
                var invoice = new Invoice                                       
                {
                    SentGreetings = group.ToList(),
                    Month = group.Key.Month,
                    Year = group.Key.Year,
                    User = user,
                };

                await _invoiceService.CreateOrUpdateInvoiceAsync(invoice);
            }
        }


    }
}
