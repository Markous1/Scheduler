using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName()
        {
            return string.IsNullOrWhiteSpace(FirstName + " " + LastName) ? this.UserName : (FirstName + " " + LastName).Trim();
        }
    }
}
