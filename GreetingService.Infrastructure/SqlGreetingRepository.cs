using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreetingService.Core;
using GreetingService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreetingService.Infrastructure
{
    public class SqlGreetingRepository : IGreetingRepository
    {
        private readonly GreetingDbContext _greetingDbContext;
        public SqlGreetingRepository(GreetingDbContext greetingDbContext)
        {
            _greetingDbContext = greetingDbContext;
        }

        public async Task CreateAsync(Greeting greeting)
        {
            await _greetingDbContext.Greetings.AddAsync(greeting);
            await _greetingDbContext.SaveChangesAsync();
        }

        public async Task DeleteRecordAsync(Guid id)
        {
            var greeting= await _greetingDbContext.Greetings.FirstOrDefaultAsync(x => x.Id == id);

            if (greeting == null)
                throw new Exception("Greeting not found");

            _greetingDbContext.Greetings.Remove(greeting);
            await _greetingDbContext.SaveChangesAsync();
        }

        public async Task<Greeting> GetAsync(Guid id)
        {
            try
            {
                var greeting = await _greetingDbContext.Greetings.FirstOrDefaultAsync(x => x.Id == id);
                return greeting;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<IEnumerable<Greeting>> GetAsync()
        {
            try
            {
                return await _greetingDbContext.Greetings.ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<IEnumerable<Greeting>> GetAsync(string from, string to)
        {
            if (!string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to))
            {
                var greetings = _greetingDbContext.Greetings.Where(x => x.From.Equals(from) && x.To.Equals(to));
                return await greetings.ToListAsync();
            }
            
            else if (!string.IsNullOrWhiteSpace(from) && string.IsNullOrWhiteSpace(to))
            {
                var greetings = _greetingDbContext.Greetings.Where(x => x.From.Equals(from));
                return await greetings.ToListAsync();
            }
            
            else if (string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to))
            {
                var greetings = _greetingDbContext.Greetings.Where(x => x.To.Equals(to));
                return await greetings.ToListAsync();
            }

            //from & to are null, return all greetings
            return await _greetingDbContext.Greetings.ToListAsync();
        }

        public async Task UpdateAsync(Greeting greeting)
        {
            var existingGreeting = await _greetingDbContext.Greetings.FirstOrDefaultAsync(x => x.Id == greeting.Id);        
            if (existingGreeting == null)
                throw new Exception("Not found");

            existingGreeting.Message = greeting.Message;                                                                        
            existingGreeting.To = greeting.To;
            existingGreeting.From = greeting.From;

            await _greetingDbContext.SaveChangesAsync();
        }
    }

    
}
