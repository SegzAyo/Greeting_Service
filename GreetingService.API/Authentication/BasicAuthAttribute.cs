using Microsoft.AspNetCore.Mvc;

namespace GreetingService.API.Authentication
{
    /// <summary>
    /// Inspiration from https://codeburst.io/adding-basic-authentication-to-an-asp-net-core-web-api-project-5439c4cf78ee
    /// This class implements a new Attribute that we will use to mark which endpoints should require Basic Authentication
    /// Almost no logic is required in this class, only basic wiring to make it usable
    /// See how we use this Attribute in GreetingController on methods with [BasicAuth]
    /// 
    /// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BasicAuthAttribute : TypeFilterAttribute
    {
        public BasicAuthAttribute(string realm = "Basic") : base(typeof(BasicAuthFilter))           //This is the important. Connect this Attribute with "BasicAuthFilter"
        {
            Arguments = new object[] { realm };
        }
    }
}