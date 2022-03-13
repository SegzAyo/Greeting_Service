using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Core.Enums
{
    /// <summary>
    /// Allowed subject values in IMessagingService. Use this enum to ensure we always send a valid subject value
    /// </summary>
    public enum MessagingServiceSubject
    {
        NewGreeting,
        UpdateGreeting,
        NewUser,
        UpdateUser,
        
    }
}