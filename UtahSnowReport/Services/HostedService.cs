using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UtahSnowReport
{
    public class HostedService : Microsoft.Extensions.Hosting.IHostedService
    {
        private readonly IHtmlService _htmlService;
        private readonly IEmailService _emailService;

        public HostedService (IHtmlService htmlService, IEmailService emailService)
        {
            _htmlService = htmlService;
            _emailService = emailService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var report = _htmlService.Build48HrReport();
            if (report.IsValid())
            {
                List<string> recipients = new List<string>
                {
                    "adam.quintana@gmail.com"
                };
                _emailService.SendAll("Utah Snow Report", report.ToHtml(), recipients);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
