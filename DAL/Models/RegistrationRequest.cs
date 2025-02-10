using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class RegistrationRequest
    {
        public string FirstName { get; set; }
        //public string MobileNumber { get; set; }
        //public string UserName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }
        //public string LastName { get; set; }
        //public string Email { get; set; }
        //public string Address { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string? CompanyName { get; set; }
    }
}
