using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ResetPasswordRequest
    {
        public string UserName { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

}
