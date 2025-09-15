using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using static System.Net.WebRequestMethods;
using System.Text.Json.Serialization;


namespace DAL.DAO
{
    public class CustomerOTP
    {
        public CustomerOTP()
        {
            CreatedOn = DateTime.Now;
            ExpireOn = CreatedOn.AddMinutes(3);

            OTP = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            //Code = Guid.NewGuid().ToString("N").Substring(0, 5);
            //CreatedOn = DateTime.Now;
            //ExpireOn = CreatedOn.Add(new TimeSpan(0, 3, 0));
        }

        public int Id { get; set; }
        public string OTP { get; set; }
        public DateTime CreatedOn { get; set; }

        //[JsonConverter(typeof(JsonStringEnumConverter))]
        public DateTime ExpireOn { get; set; }

        //public string Code { get; set; }
        public string PhoneNumber { get; set; }
        public int OTPType { get; set; }

        //[ForeignKey("Customer")]
        //public string CustomerId { get; set; }
        //public Customer? Customers { get; set; }

    }
}
