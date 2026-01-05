using HigzTrade.Infrastructure.ExternalServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Infrastructure.BackgroundJobs
{
    public sealed class SendApplicationErrorEmailJob
    {
        private readonly MailService _mailService;
        private readonly ILogger<SendApplicationErrorEmailJob> _log;

        public SendApplicationErrorEmailJob(
            MailService mailService,
            ILogger<SendApplicationErrorEmailJob> log)
        {
            _mailService = mailService;
            _log = log;
        }

        public async Task ExecuteAsync(
            string traceId,
            int status,
            string title,
            string message,
            string stackTrace)
        {
            try
            {
                await _mailService.SendApplicationErrorEmailAsync(
                    traceId,
                    status,
                    title,
                    message,
                    stackTrace);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to send application error email. TraceId={TraceId}",traceId);
                throw; // ให้ Hangfire retry
            }
        }
    }

}
