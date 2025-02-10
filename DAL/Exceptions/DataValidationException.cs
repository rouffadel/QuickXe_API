using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Exceptions
{
    public class DataValidationException : DomainException
    {
        public DataValidationException(string message) : base(message)
        {
        }
    }
}
