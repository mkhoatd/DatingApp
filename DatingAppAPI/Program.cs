global using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using DatingAppAPI.Data;
using Serilog;
using Serilog.Events;
using Serilog.AspNetCore;
using Serilog.Sinks.Elasticsearch;
using DatingAppAPI.Interfaces;
using DatingAppAPI.Services;
using DatingAppAPI.Extensions;

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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

app.UseAuthentication();

app.UseAuthorization();

app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
