using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DAO
{
    public class CountriesMaster
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string? CountryCode { get; set; }
        public string? CurrencyName { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencySymbol { get; set; }
    }
}
