using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Vitahus_VideoService_Service.RabbitMQ;
public interface IRabbitMQService
{
    bool SendMessage(string queueName, string message);
}
public class RabbitMQService : IRabbitMQService
{
    private readonly IConnectionFactory _connectionFactory;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly ILogger<RabbitMQService>? _logger;

    public RabbitMQService(IConnectionFactory connectionFactory, ILogger<RabbitMQService> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        CreateConnection();
    }

    private void CreateConnection()
    {
        try
        {
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
        }
        catch (Exception ex)
        {
            _logger?.LogError($"Kunne ikke oprette forbindelse til RabbitMQ: {ex.Message}\n");
        }
    }

    public bool SendMessage(string queueName, string message)
    {
        try
        {
            EnsureConnection();

            _channel?.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            
            _channel?.ExchangeDeclare("directLogging", ExchangeType.Direct);
            var body = Encoding.UTF8.GetBytes(message);
            _channel?.QueueBind(queue: queueName, exchange: "directLogging", routingKey: queueName);
            _channel?.BasicPublish(exchange: "directLogging", routingKey: queueName, basicProperties: null, body: body);
            _logger?.LogInformation($"Sendt besked til {queueName}: {message}\n");
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError($"Fejl ved afsendelse af besked: {ex.Message}\n");
            return false;
        }
    }

    private void EnsureConnection()
    {
        if (_connection is not { IsOpen: true })
        {
            CreateConnection();
        }

        if (_channel == null || _channel.IsClosed)
        {
            _channel = _connection?.CreateModel();
        }
    }
}