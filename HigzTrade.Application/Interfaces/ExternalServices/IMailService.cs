using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Application.Interfaces.ExternalServices
{
    public interface IMailService
    {
        Task<bool> SendApplicationErrorEmailAsync(string traceId, int httpStatusCode, string title, string message, string stackTrace);
    }
}
