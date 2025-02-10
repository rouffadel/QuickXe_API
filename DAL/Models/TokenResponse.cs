using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DAO;

namespace DAL.Models
{
    public class TokenResponse
    {
        public string Token { get; set; } = string.Empty;

        public ApplicationUser User { get; set; }

        public bool RegionConfirm { get; set; }

        public int? PlanType { get; set; }
        public DateTimeOffset Expiration { get; set; } = DateTimeOffset.MinValue;

    }
    public class Response
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
