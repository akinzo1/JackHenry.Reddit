
using Reddit.API.Repository;
using Reddit.API;
using System.Reflection;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        //var services = builder.Services;
        
        builder.AddRedisClient("redis");
        
        builder.Services.AddScoped<IRedditRepository, RedisRedditRepository>();

        builder.Services.AddHttpContextAccessor();

        //builder.Services.AddSwaggerGen();

        //// Pooling is disabled because of the following error:
        //// Unhandled exception. System.InvalidOperationException:
        //// The DbContext of type 'OrderingContext' cannot be pooled because it does not have a public constructor accepting a single parameter of type DbContextOptions or has more than one constructor.
        //services.AddDbContext<OrderingContext>(options =>
        //{
        //    options.UseNpgsql(builder.Configuration.GetConnectionString("orderingdb"));
        //});
        //builder.EnrichNpgsqlDbContext<OrderingContext>();

        //services.AddMigration<OrderingContext, OrderingContextSeed>();

        // Add the integration services that consume the DbContext
        //services.AddTransient<IIntegrationEventLogService, IntegrationEventLogService<OrderingContext>>();

        //services.AddTransient<IOrderingIntegrationEventService, OrderingIntegrationEventService>();

        //builder.AddRabbitMqEventBus("eventbus")
        //       .AddEventBusSubscriptions();


        //// Configure mediatR
        builder.Services.AddMediatR(cfg =>
        {
            //cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.RegisterServicesFromAssemblyContaining(typeof(Program));

            //cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            //cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
            //cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        //// Register the command validators for the validator behavior (validators based on FluentValidation library)
        //services.AddSingleton<IValidator<CancelOrderCommand>, CancelOrderCommandValidator>();
        //services.AddSingleton<IValidator<CreateOrderCommand>, CreateOrderCommandValidator>();
        //services.AddSingleton<IValidator<IdentifiedCommand<CreateOrderCommand, bool>>, IdentifiedCommandValidator>();
        //services.AddSingleton<IValidator<ShipOrderCommand>, ShipOrderCommandValidator>();

        //services.AddScoped<IOrderQueries, OrderQueries>();
        //services.AddScoped<IBuyerRepository, BuyerRepository>();
        //services.AddScoped<IOrderRepository, OrderRepository>();
        //services.AddScoped<IRequestManager, RequestManager>();
    }

}
