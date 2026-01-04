using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Domain.Exceptions
{

    public class DomainException : Exception
    {
        public DomainException(string message, Exception? inner = null) : base(message, inner) { }
    }
}
