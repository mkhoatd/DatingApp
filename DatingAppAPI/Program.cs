using Microsoft.EntityFrameworkCore;
using DatingAppAPI.Data;
using Serilog;
using Serilog.Events;
using Serilog.AspNetCore;
using Serilog.Sinks.Elasticsearch;

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

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlite(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
