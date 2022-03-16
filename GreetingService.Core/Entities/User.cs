using GreetingService.Core.Helper_Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Core
{
    public class User
    {
        public string firstName { get; set; }
        public string lastName { get; set; }

        private string _email;
        public string email
        {
            get
            {
                return _email;
            }
            set
            {
                if (!EmailAuth.IsValid(value))
                    throw new Exception($"{value} is not a valid email");

                _email = value;
            }
        } 
        public string password { get; set; }    
        public DateTime created { get; set; } = DateTime.Now;
        public DateTime modified  { get; set; } =DateTime.Now;
        public UserApprovalStatus ApprovalStatus { get; set; }
        public string ApprovalCode { get; set; } = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)).Replace("/", "").Replace("?", "");
        public string ApprovalStatusNote { get; set; }
        public DateTime ApprovalExpiry { get; set; } = DateTime.Now.AddHours(6);

        public enum UserApprovalStatus
        {
            Approved = 0,
            Rejected = 1,
            Pending = 2,
        }
    }
}
