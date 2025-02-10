using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class LoginResponse
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ValidTo { get; set; }
        public List<Roles> Roles { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
    }
}
