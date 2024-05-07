﻿
using Reddit.API.Repository;
using Reddit.API;
using System.Reflection;
using EventBusRabbitMQ;
using EventBus.Extensions;
using Reddit.API.Application.IntegrationEvents.Events;
using Reddit.API.Application.IntegrationEvents.EventHandling;
using Reddit.API.Model.Configuration;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        
        builder.AddRedisClient("redis", null, configureOptions =>
        {
            configureOptions.ConnectTimeout = 100000;
        });

        services.AddScoped<IRedditRepository, RedisRedditRepository>();

        services.Configure<RedditSettings>(builder.Configuration.GetSection("RedditSettings"));

        services.AddHttpContextAccessor();

        builder.Services.AddSwaggerGen();

        //add message broker
        builder.AddRabbitMqEventBus("eventbus")
            .AddSubscription<UpdateRedditRequestIntegrationEvent, UpdateRedditRequestIntegrationEventHandler>();

        // Configure mediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining(typeof(Program));
        });

    }

    //Convert to DateTime
    public static DateTime UnixSecondsToDateTime(long timestamp, bool local = false)
    {
        var offset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        return local ? offset.LocalDateTime : offset.UtcDateTime;
    }

    public static DateTime UnixMillisecondsToDateTime(long timestamp, bool local = false)
    {
        var offset = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
        return local ? offset.LocalDateTime : offset.UtcDateTime;
    }

}
