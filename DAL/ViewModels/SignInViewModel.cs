using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class SignInViewModel
    {
        //[Required,EmailAddress]
        [Required]
        public string? EmailAddressOrPhoneNumber { get; set; }
        
        [Required]
        public string? Password { get; set; }

    }
}
