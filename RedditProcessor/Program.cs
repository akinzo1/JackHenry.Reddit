using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Builder;
using RedditProcessor.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddApplicationServices();

var host = builder.Build();

host.MapDefaultEndpoints();

host.Run();
