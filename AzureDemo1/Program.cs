using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using AzureDemo1.repository;
using Microsoft.EntityFrameworkCore; 
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
string? storageConnectionString = builder.Configuration["AzureStorage:ConnectionString"];
string? queueName = builder.Configuration["AzureStorage:QueueName"];

// It's good practice to ensure the connection string exists
if (string.IsNullOrEmpty(connectionString))
{
    // You might want more robust error handling or logging here
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
}
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

// Option 1: Register QueueServiceClient (if you interact with multiple queues)
//builder.Services.AddSingleton(x => new QueueServiceClient(storageConnectionString));

// Option 2: Register specific QueueClient (if mainly using one queue)
builder.Services.AddSingleton(x => new BlobServiceClient(storageConnectionString));

builder.Services.AddSingleton(x => new QueueClient(storageConnectionString, queueName));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => // Configure Swagger Generator
{ 
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Demo Azure API", // Customize this
        Description = "An ASP.NET Core Web API for managing Items and Files", // Customize
    });

    // --- Configuration from items below will go inside this AddSwaggerGen block ---
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => // Serves the Swagger UI HTML page
    { 
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1"); 
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
