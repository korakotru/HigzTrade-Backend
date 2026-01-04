using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Application.Exceptions
{

    public class BusinessException : Exception
    {
        public BusinessException(string message, Exception? inner = null) : base(message, inner) { }
    }
}
