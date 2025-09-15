using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public record OTPRequest
    {
        public string PhoneNumber { get; set; }
        public string OTP { get; set; }
    }
}
