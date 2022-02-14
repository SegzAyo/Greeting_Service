using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Net;
using GreetingService.Infrastructure;
using GreetingService.Core;

namespace GreetingService.API.Authentication
{
    /// <summary>
    /// Inspiration from https://codeburst.io/adding-basic-authentication-to-an-asp-net-core-web-api-project-5439c4cf78ee
    /// 
    /// An IAuthorizationFilter is executed in the ASP.NET pipeline before the execution reaches our own code.
    /// A nice thing with ASP.NET is that it's possible to add our own custom logic to the pipeline. API authentication logic is a great example.
    /// Implementing IAuthorizationFilter requires only one method: OnAuthorization()
    /// This method is called for all incoming requests to methods with our custom attribute [BasicAuth]
    /// 
    /// </summary>
    public class BasicAuthFilter : IAuthorizationFilter
    {
        private readonly string _realm;
        private readonly IUserService _userService;

        public BasicAuthFilter(string realm, IUserService userService)
        {
            _realm = realm;
            _userService = userService;
            if (string.IsNullOrWhiteSpace(_realm))
            {
                throw new ArgumentNullException(nameof(realm), @"Please provide a non-empty realm value.");
            }
        }

        /// <summary>
        /// Extract the HTTP header: Authorization and decode the value to extract username and password.
        /// Call the configured implementation i IUSerService to validate credentials
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                string authHeader = context.HttpContext.Request.Headers["Authorization"];
                if (authHeader != null)
                {
                    var authHeaderValue = AuthenticationHeaderValue.Parse(authHeader);
                    if (authHeaderValue.Scheme.Equals(AuthenticationSchemes.Basic.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        var credentials = Encoding.UTF8
                                            .GetString(Convert.FromBase64String(authHeaderValue.Parameter ?? string.Empty))         //decode base64encoded string to normal strings to parse username:password - don't confuse this with encryption, anybody with access to the encoded string can decode it like this
                                            .Split(':', 2);
                        if (credentials.Length == 2)
                        {
                            if (IsAuthorized(credentials[0], credentials[1]))
                            {
                                return;
                            }
                        }
                    }
                }

                ReturnUnauthorizedResult(context);
            }
            catch (FormatException)
            {
                ReturnUnauthorizedResult(context);
            }
        }

        public bool IsAuthorized(string username, string password)
        {
            return _userService.IsValidUser(username, password);
        }

        private void ReturnUnauthorizedResult(AuthorizationFilterContext context)
        {
            // Return 401 and a basic authentication challenge (causes browser to show login dialog)
            context.HttpContext.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{_realm}\"";
            context.Result = new UnauthorizedResult();
        }
    }
}