global using System.Text.Json;
global using System.Text.Json.Serialization;
global using AutoMapper;
global using System.Security.Cryptography;
global using System.Text;
using Microsoft.EntityFrameworkCore;
using DatingAppAPI.Data;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using DatingAppAPI.Extensions;
using DatingAppAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var elasticsearchUri = builder.Configuration["ElasticSearch:Uri"];

builder.Host.UseSerilog((context, configuration) => configuration
    .WriteTo.Elasticsearch(
        new ElasticsearchSinkOptions(new Uri(elasticsearchUri))
        {
            IndexFormat = $"{context.HostingEnvironment.ApplicationName}-logs-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.Now:yyyy-MM}",
            AutoRegisterTemplate = true,
            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7
        }
    )
    .ReadFrom.Configuration(context.Configuration));
// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
builder.Services.AddCors();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DataContext>();
        await context.Database.MigrateAsync();
        await Seed.SeedUsers(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occured during migration");
    }
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

app.UseAuthentication();

app.UseAuthorization();

app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
