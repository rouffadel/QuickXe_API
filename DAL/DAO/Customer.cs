using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using static System.Net.WebRequestMethods;

namespace DAL.DAO
{
    public class Customer
    {
        public Customer()
        {
            CustomerId = Guid.NewGuid().ToString();
        }

        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        //public List<CustomerOTP> CustomerOTPs { get; set; }


    }
}
