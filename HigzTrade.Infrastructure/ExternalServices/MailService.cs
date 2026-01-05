using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Infrastructure.ExternalServices
{
    public class MailService
    {
        //todo:implement email services
        public async Task<bool> SendApplicationErrorEmailAsync(string traceId, int httpStatusCode, string title, string message, string stackTrace)
        {
            //todo:send error message to support
            await Task.Delay(100000);
            return true;
        }
    }
}
