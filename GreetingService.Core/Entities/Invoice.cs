using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Core.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        public User User { get; set; }
        public List<Greeting> SentGreetings { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

        public double CostPerGreeting = 2.0;
        private double _totalCost;
        public double TotalCost
        {
            get
            {
                return _totalCost; 
            }
            set 
            { 
                _totalCost = SentGreetings.Count * CostPerGreeting; 
            
            }
        }
        public string Currency { get; set; }
    }
}
