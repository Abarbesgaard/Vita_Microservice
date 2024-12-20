using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Vitahus_VideoService_Shared;

namespace Vitahus_VideoService_Service.RabbitMQ;

public class AuditLogConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IAuditLogService _auditLogService;


    public AuditLogConsumer(IConnectionFactory connectionFactory, IAuditLogService auditLogService)
    {
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _auditLogService = auditLogService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel.ExchangeDeclare(
            exchange: "directLogging",
            type: ExchangeType.Direct,
            durable: false);

        _channel.QueueDeclare(
            queue: "auditLogQueue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        _channel.QueueBind(
            queue: "auditLogQueue",
            exchange: "directLogging",
            routingKey: "auditLogQueue");

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Received message: {message}");

            try
            {
                var request = JsonSerializer.Deserialize<AuditLog>(message);
                if (request != null)
                {
                    Console.WriteLine("Behandler AuditLog anmodning");
                    await Task.Delay(1000 + new Random().Next(3000));
                    await _auditLogService.LogAsync(request);

                }
                _channel?.BasicAck(ea.DeliveryTag, false);
                Console.WriteLine("Besked behandlet og bekræftet");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Fejl ved deserialisering af besked: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl ved håndtering af besked: {ex.Message}");
                _channel?.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(
            queue: "auditLogQueue",
            autoAck: false,
            consumer: consumer);

        await Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
