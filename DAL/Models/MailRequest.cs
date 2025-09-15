using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class MailRequest
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ToEmail { get; set; }
    }

}
