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
        public string? RoleId { get; set; }
        public string? Country { get; set; }
        public string? State {  get; set; }
        public string? District { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? PersonalVisitForRegistration { get; set; }
        public bool EmailStatus { get; set; }
        public string? IsActive { get; set; }
        public string? ActivationDate { get; set; }
        public List<Country> Countries { get; set; }
        public List<SendEmail> SendEmails { get; set; }

    }

}
