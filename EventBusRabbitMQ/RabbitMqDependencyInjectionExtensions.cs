
namespace EventBusRabbitMQ;

public static class RabbitMqDependencyInjectionExtensions
{
    // {
    //   "EventBus": {
    //     "SubscriptionClientName": "...",
    //     "RetryCount": 10
    //   }
    // }

    private const string SectionName = "EventBus";

    public static IEventBusBuilder AddRabbitMqEventBus(this IHostApplicationBuilder builder, string connectionName)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AddRabbitMQClient(connectionName, configureSettings: settings =>
        {
            settings.ConnectionString = "amqp://localhost:5672"; // use 127.0.0.1 if IP not resolving to localhost 
        }, configureConnectionFactory: factory =>
        {
            ((ConnectionFactory)factory).DispatchConsumersAsync = true;
        });

        // Options support
        builder.Services.Configure<EventBusOptions>(builder.Configuration.GetSection("EventBus"));

        // Abstractions on top of the core client API
        builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();

        // Start consuming messages as soon as the application starts
        builder.Services.AddSingleton<IHostedService>(sp => (RabbitMQEventBus)sp.GetRequiredService<IEventBus>());

        return new EventBusBuilder(builder.Services);

    }

    private class EventBusBuilder(IServiceCollection services) : IEventBusBuilder
    {
        public IServiceCollection Services => services;
    }
}
