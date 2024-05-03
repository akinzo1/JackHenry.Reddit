
using Reddit.API;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.AddApplicationServices();

builder.Services.AddControllers();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
