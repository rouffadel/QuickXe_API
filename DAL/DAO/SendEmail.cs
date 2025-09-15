using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DAO
{
    public class SendEmail
    {
        public SendEmail()
        {
            EmailCode = Guid.NewGuid().ToString("N").Substring(0, 5);

        }
        public int Id { get; set; }
        public string EmailCode { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

    }
}
