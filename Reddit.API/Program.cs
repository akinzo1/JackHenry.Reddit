
using Reddit.API;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.AddApplicationServices();

var withApiVersioning = builder.Services.AddApiVersioning();
builder.AddDefaultOpenApi(withApiVersioning);

var app = builder.Build();
app.MapDefaultEndpoints();
var reddit = app.NewVersionedApi("Reddit");
reddit.MapRedditApiV1();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCompression();

app.MapHub<NotificationsHub>("/reddithub");

app.UseHttpsRedirection();

app.Run();
