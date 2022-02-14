using Microsoft.AspNetCore.Http;

namespace GreetingService.API.Function.Authentication
{
    public interface IAuthHandlerBase
    {
        public bool IsAuthorized(HttpRequest req);
    }
}