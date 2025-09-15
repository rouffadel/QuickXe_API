using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class UpdateUserDTO
    {
        public string? ContactName { get; set; }
        public string? ContactNo { get; set; }
        public string? CompanyName { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? District { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude {  get; set; }
    }

}
