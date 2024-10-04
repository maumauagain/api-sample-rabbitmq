using API.Models;
using MassTransit;

namespace API.Events;

public class ProcessedReportEventConsumer : IConsumer<ProcessedReportEvent>
{
    private readonly ILogger<ProcessedReportEventConsumer> _logger;
    public ProcessedReportEventConsumer(ILogger<ProcessedReportEventConsumer> logger)
    {
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<ProcessedReportEvent> context)
    {
        _logger.LogInformation($"Inicio Processamento Relatorio ID: {context.Message.ReportId}");
        var report = ReportList.Reports.FirstOrDefault(r => r.Id == context.Message.ReportId);

        //simulate a processing
        await Task.Delay(TimeSpan.FromSeconds(5));
        if(report != null)
        {
            report.Status = "Processado";
        }
        _logger.LogInformation($"Fim Processamento Relatorio ID: {context.Message.ReportId}");
    }
}
