using GreetingService.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure
{
    public class SqlUserService : IUserService
    {
        private readonly GreetingDbContext _greetingDbContext;
        //private readonly logger<ILogger>
        public SqlUserService(GreetingDbContext greetingDbContext)
        {
            _greetingDbContext = greetingDbContext;
        }

        public bool IsValidUser(string username, string password)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> IsValidUserAsync(string username, string password)
        {
            var user = _greetingDbContext.Users;

            if (user == null)
                return false;

            try
            {
                var selectedUser = user.FirstOrDefault(u => u.email == username);
                if (selectedUser.password != password)
                    return false;

                return true;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        public async Task<User> GetUserAsync(string email)
        {
            var user = _greetingDbContext.Users.FirstOrDefault(u => u.email == email);
            if (user == null)
                throw new Exception("User is not valid");
            return user;
       
        }
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var users = _greetingDbContext.Users.ToList();
            return users;
        }

        public async Task CreateUserAsync(User user)
        {
            user.created = DateTime.Now;
            user.modified = DateTime.Now;
            _greetingDbContext.Users.Add(user);
            _greetingDbContext.SaveChanges();
        }

        public async Task UpdateUserAsync(User user)
        {
            var existingUser = _greetingDbContext.Users.FirstOrDefault(u => u.email == user.email);
            if (existingUser == null)
                throw new Exception("The user is null or does not exist");
            existingUser.modified = DateTime.Now;
            existingUser.firstName = user.firstName;
            existingUser.lastName = user.lastName;
            existingUser.password = user.password;
            _greetingDbContext.SaveChanges();
        }

        public async Task DeleteUserAsync(string email)
        {
            var user = _greetingDbContext.Users.FirstOrDefault(u => u.email == email);
            if (user == null)
                throw new Exception("There was a conflict");
            _greetingDbContext.Users.Remove(user);
            await _greetingDbContext.SaveChangesAsync();
        }

        public Task DeleteUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
