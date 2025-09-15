using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public record CreateCustomerOTPDTO
    {
        //[Required]
        //[UniqueCountryName]
        //public string EmailCode { get; set; }
        //public string CustomerId { get; set; }
        public string PhoneNumber { get; set; }
        public int OTPType { get; set; }

    }
}
