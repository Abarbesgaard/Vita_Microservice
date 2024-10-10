using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Vitahus_VideoService_Repository;
using Vitahus_VideoService_Shared;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Vitahus_VideoService_Service.RabbitMQ;

public class VideoConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IVideoService _videoService;

    private readonly ILogger<VideoConsumer>? _logger;
    public VideoConsumer(IConnectionFactory connectionFactory, IVideoService videoService, ILogger<VideoConsumer> logger)
    {
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _videoService = videoService;
    }

    protected override  async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger?.LogInformation("Modtaget Besked: {Message}\n", message);

            try
            {
                var request = JsonSerializer.Deserialize<VideoRequest>(message);
                if (request != null && request.Action == "GetAll")
                {
                    _logger?.LogInformation("Behandler GetAll anmodning\n");
                    var videos = await _videoService.GetVideosAsync();
                } 
                _channel?.BasicAck(ea.DeliveryTag, false);
                _logger?.LogInformation("Besked behandlet og bekræftet\n");
            }
            catch (JsonException ex)
            {
                _logger?.LogError("Fejl ved deserialisering af besked: {Message}", ex.Message);
            }
            catch (Exception ex)
            {
                _logger?.LogError("Fejl ved håndtering af besked: {Message}", ex.Message);
                _channel?.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(queue: "videoQueue", autoAck: false, consumer: consumer);
        await Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}

public class VideoRequest
{
    public Guid RequestId { get; set; }
    public string Action { get; set; } = string.Empty;
}