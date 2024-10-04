using API.Events;
using API.Models;
using MassTransit;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var rabbitMQUrl = builder.Configuration?.GetValue<string>("RabbitMQ:Url");
var rabbitMQUsername = builder.Configuration?.GetValue<string>("RabbitMQ:Username");
var rabbitMQPassword = builder.Configuration?.GetValue<string>("RabbitMQ:Password");

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProcessedReportEventConsumer>();
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(rabbitMQUrl, host =>
        {
            host.Username(rabbitMQUsername!);
            host.Password(rabbitMQPassword!);
        });
        
        cfg.ReceiveEndpoint("minha-fila", e =>
        {
            e.ConfigureConsumeTopology = false;
            e.ConfigureConsumer<ProcessedReportEventConsumer>(ctx);
            e.Bind("minha-exchange", x =>
            {
                x.ExchangeType = "direct";
                x.RoutingKey = "minha-rota";
            });
        });

        cfg.ConfigureEndpoints(ctx);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/reports", async (string name, IPublishEndpoint bus, ISendEndpointProvider endpoint) =>
{
    var queue = await endpoint.GetSendEndpoint(new Uri("queue:minha-fila")); 
    var report = new Report() { Name = name};
    ReportList.Reports.Add(report);
    var msg = new ProcessedReportEvent(report.Id);
    //await bus.Publish(msg);
    await queue.Send<ProcessedReportEvent>(msg);
    return Results.Ok(report);
});

app.MapGet("/reports", () => ReportList.Reports.ToArray());


app.Run();
