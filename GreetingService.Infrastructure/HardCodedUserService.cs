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

        public Task ApproveUserAsync(string approvalCode)
        {
            throw new NotImplementedException();
        }

        public Task CreateUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAsync(string emaail)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetUsersAsync()
        {
            throw new NotImplementedException();
        }

        public bool IsValidUser(string username, string password)
        {
            if (!_users.TryGetValue(username, out var storedPassword))              //user does not exist
                return false;

            if (!storedPassword.Equals(password))
                return false;

            return true;
        }

        public Task<bool> IsValidUserAsync(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task RejectUserAsync(string approvalCode)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}