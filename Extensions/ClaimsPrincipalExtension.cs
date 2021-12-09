using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Scheduler.Extensions
{
    public static class ClaimsPrincipalExtension
    {

        public static string GetFirstName(this ClaimsPrincipal principal)
        {
            var fullName = principal.Claims.FirstOrDefault(c => c.Type == "FirstName");
            return fullName?.Value;
        }

        public static string GetLastName(this ClaimsPrincipal principal)
        {
            var fullName = principal.Claims.FirstOrDefault(c => c.Type == "LastName");
            return fullName?.Value;
        }
    }
}
