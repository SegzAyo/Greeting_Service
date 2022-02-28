using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Core
{
    public class User
    {
        public string firstName{ get; set; }
        public string lastName{ get; set; }
        public string email{ get; set; }
        public string password { get; set; }    
        public DateTime created { get; set; }
        public DateTime modified  { get; set; }
    }
}
