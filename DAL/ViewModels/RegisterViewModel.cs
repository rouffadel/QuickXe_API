using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace DAL.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string? FullName { get; set; }

        //[Required, EmailAddress]
        [Required]
        public string? EmailAddressOrPhoneNumber { get; set; }

        [Required]
        public string? Password { get; set; }

        //[Compare("Password")]
        //public string? ConfirmPassword { get; set; }
        public string? Company { get; set; }
    }
}
