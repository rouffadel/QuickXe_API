using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class TokenModel
    {
        public string userName { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
