namespace GreetingService.Infrastructure
{
    public interface IUserService
    {
        bool IsValidUser(string username, string password);
    }
}