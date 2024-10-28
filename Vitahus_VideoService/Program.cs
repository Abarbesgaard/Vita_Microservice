using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using Vitahus_VideoService.Validation;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Serilog;
using Vitahus_VideoService_Data;
using Vitahus_VideoService_Repository;
using Vitahus_VideoService_Service;
using Vitahus_VideoService_Shared;
using Vitahus_VideoService.Mapping;
using Vitahus_VideoService_Service.RabbitMQ;
using RabbitMQ.Client;

namespace Vitahus_VideoService;

public static class Program
{
    public static Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
        var mapper = mapperConfig.CreateMapper();
        mapper.ConfigurationProvider.AssertConfigurationIsValid();


        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton(mapper);



        builder.Services.AddControllers(options => { options.SuppressAsyncSuffixInActionNames = false; });

        var mongoDbSettingsSection = builder.Configuration.GetSection("MongoDbSettings");
        builder.Services.Configure<MongoDbSettings>(mongoDbSettingsSection);

        builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var mongoDbSettings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return new MongoClient(mongoDbSettings.ConnectionString);
        });

        builder.Services.AddSingleton<IMongoDatabase>(serviceProvider =>
        {
            var mongoDbSettings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoDbSettings.DatabaseName);
        });
        builder.Services.AddSingleton(serviceProvider =>
        {
            var database = serviceProvider.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<AuditLog>("AuditLogs");  // Ensure this matches your collection name
        });

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                corsPolicyBuilder =>
                {
                    corsPolicyBuilder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
        builder.Services.AddScoped<IValidator<Video>, VideoValidator>();
        builder.Services.AddScoped<IValidator<BaseEntity>, BaseEntityValidator>();

        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(typeof(ObjectId), new ObjectSerializer());

        builder.Services.AddSingleton(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        builder.Services.AddSingleton<IVideoService, VideoService>();
        builder.Services.AddSingleton<IAuditLogService, AuditLogService>();
        builder.Services.AddSingleton<IConnectionFactory>(sp =>
        new ConnectionFactory()
        {
            HostName = "localhost", // eller din RabbitMQ server adresse
            UserName = "guest", // eller dit brugernavn
            Password = "guest"  // eller din adgangskode
        });
        builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();

        builder.Services.AddHostedService<VideoConsumer>();
        builder.Services.AddHostedService<AuditLogConsumer>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
         {
             options.SwaggerDoc("v1", new OpenApiInfo { Title = "Vitahus_VideoService", Version = "v1" });
         });

        var app = builder.Build();
        app.Urls.Add("https://localhost:5001");
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vitahus_VideoService v1"));
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors();
        app.MapControllers();

        app.Run();
        return Task.CompletedTask;
    }
}
