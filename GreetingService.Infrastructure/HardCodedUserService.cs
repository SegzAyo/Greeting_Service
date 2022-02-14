using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreetingService.Core;

namespace GreetingService.Infrastructure
{
    public class HardCodedUserService : IUserService
    {
        private static IDictionary<string, string> _users = new Dictionary<string, string>()
        {
            { "keen","summer2022" },
            { "anton","winter2022" },
        };
        public bool IsValidUser(string username, string password)
        {
            if (!_users.TryGetValue(username, out var storedPassword))              //user does not exist
                return false;

            if (!storedPassword.Equals(password))
                return false;

            return true;
        }
    }
}