using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HigzTrade.Domain.Exceptions
{

    public class BusinessException : Exception
    {
        public List<string> Errors { get; } = new();
        public BusinessException(string message, Exception? inner = null) : base(message, inner) { }
        public BusinessException(List<string> errors, Exception? innerException = null) : base("Business rules invalid", innerException)
        {
            Errors = errors;
        }
    }
}
