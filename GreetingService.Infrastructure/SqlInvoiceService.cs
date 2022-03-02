using GreetingService.Core.Entities;
using GreetingService.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure
{
    public class SqlInvoiceService : IInvoiceService
    {
        private readonly GreetingDbContext _greetingDbContext;

        public SqlInvoiceService(GreetingDbContext greetingDbContext)
        {
      
            _greetingDbContext = greetingDbContext;

        }

        public async Task CreateOrUpdateInvoiceAsync(Invoice invoice)
        {
            var checkInvoice = _greetingDbContext.Invoices.FirstOrDefault(inv => inv.Month == invoice.Month && inv.Year == invoice.Year && inv.User.email.Equals(invoice.User.email));
            if (checkInvoice == null)
            {
                await _greetingDbContext.AddAsync(invoice);
                await _greetingDbContext.SaveChangesAsync();
            }
            else
            {
                checkInvoice.SentGreetings = invoice.SentGreetings;
                checkInvoice.TotalCost = invoice.TotalCost;
                await _greetingDbContext.SaveChangesAsync();
            }
        }

        public async Task<Invoice> GetInvoiceAsync(int year, int month, string email)
        {
            var checkInvoice = _greetingDbContext.Invoices.FirstOrDefault(inv => inv.Year == year && inv.Month == month && inv.User.email == email);
            if (checkInvoice == null)
                throw new Exception("Invoice not found");

            return checkInvoice;
      
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesAsync(int year, int month)
        {
            var checkedInvoices = await _greetingDbContext.Invoices
                .Include(inv => inv.SentGreetings)
                .Include(inv => inv.User)
                .Where(inv => inv.Year == year && inv.Month == month).ToListAsync();


            return checkedInvoices;
        }
    }
}
