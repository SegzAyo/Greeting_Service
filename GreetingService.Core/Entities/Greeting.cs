using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Core.Entities
{
    public class Greeting
    {
        public Guid Id { get; set; } = Guid.NewGuid(); 
        public string Message { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Timestamp { get; set; }
        
    }

    
}



