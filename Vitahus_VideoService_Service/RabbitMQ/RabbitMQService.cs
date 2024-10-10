using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
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

            var body = Encoding.UTF8.GetBytes(message);

            _channel?.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
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
        if (_connection == null || !_connection.IsOpen)
        {
            CreateConnection();
        }

        if (_channel == null || _channel.IsClosed)
        {
            _channel = _connection?.CreateModel();
        }
    }
}