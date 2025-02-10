using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DAL.DAO
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {

        }
        public string ContactName { get; set; }
        public string? ContactNo { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime LastPasswordChangedDate { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime PasswordRetrieveDate { get; set; }
        public DateTime LastLockoutDate { get; set; }
        public DateTime FailedPasswordAttemptWindowStart { get; set; }
        public string? CompanyName { get; set; }
    }

}
