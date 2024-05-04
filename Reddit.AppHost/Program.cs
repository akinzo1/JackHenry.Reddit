
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");


builder.AddProject<Projects.Reddit_API>("reddit-api")
    .WithReference(redis);
    

builder.AddProject<Projects.RedditProcessor>("redditprocessor");

builder.Build().Run();
