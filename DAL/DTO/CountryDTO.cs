//using DAL.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public record CreateCountryDTO
    {
        [Required]
        //[UniqueCountryName]
        public string? CountryName { get; set; }
        public string? CountryCode { get; set; }
        public string? CurrencyName { get; set; }
        public double? BuyRate { get; set; }
        public double? SellRate { get; set; }
        public string? TenantId { get; set; }

    }
}
