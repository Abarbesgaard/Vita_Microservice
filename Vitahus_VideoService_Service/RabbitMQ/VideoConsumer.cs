using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Vitahus_VideoService_Shared;

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
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override  async Task ExecuteAsync(CancellationToken stoppingToken)
    {
 _channel.QueueDeclare(queue: "videoQueue",
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);
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
                    // Simulerer en tungere opgave ved at tilføje en tilfældig ventetid
                    var stopwatch = Stopwatch.StartNew();
                    await Task.Delay(1000 + new Random().Next(2000));
                    stopwatch.Stop();
                    _logger?.LogInformation($"[TIME] GetAll anmodning behandlet efter {stopwatch.ElapsedMilliseconds} ms\n");

                    var videos = await _videoService.GetVideosAsync();
                } 
                else if (request != null && request.Action == "GetById")
                {
                    _logger?.LogInformation("Behandler GetById anmodning\n");
                    var stopwatch = Stopwatch.StartNew();
                    await Task.Delay(1000 + new Random().Next(2000));
                    stopwatch.Stop();
                    _logger?.LogInformation($"[TIME] GetById anmodning behandlet efter {stopwatch.ElapsedMilliseconds} ms\n");

                    var video = await _videoService.GetVideoAsync(request.RequestId);
                }
                else if (request != null && request.Action == "Create")
                {
                    _logger?.LogInformation("Behandler Create anmodning\n");
                    var video = JsonSerializer.Deserialize<Video>(message);
                    if (video != null)
                    {
                        var stopwatch = Stopwatch.StartNew();
                        await Task.Delay(1000 + new Random().Next(2000));

                        await _videoService.AddVideoAsync(video);
                        stopwatch.Stop();
                        _logger?.LogInformation($"[TIME] Create anmodning behandlet efter {stopwatch.ElapsedMilliseconds} ms\n");
                    }
                    else
                    {
                        _logger?.LogError("Fejl ved deserialisering af video\n");
                    }
                }
                else
                {
                    _logger?.LogError("Ugyldig anmodning\n");
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