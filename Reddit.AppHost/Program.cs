
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");
var rabbitMq = builder.AddRabbitMQ("eventbus");


builder.AddProject<Projects.Reddit_API>("reddit-api")
    .WithReference(redis)
    .WithReference(rabbitMq);
    
builder.AddProject<Projects.RedditProcessor>("redditprocessor")
    .WithReference(rabbitMq);

builder.Build().Run();
