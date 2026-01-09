using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Infrastructure.Exceptions
{

    public class InfrastructureException : Exception
    {
        public InfrastructureException(string message, Exception? inner = null) : base(message, inner) { }
    }
}
