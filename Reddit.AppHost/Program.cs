
var builder = DistributedApplication.CreateBuilder(args);

builder.AddRedis("cache");


builder.AddProject<Projects.Reddit_API>("reddit-api");

builder.AddProject<Projects.RedditProcessor>("redditprocessor");

builder.Build().Run();
