using AzureDemo1.repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// It's good practice to ensure the connection string exists
if (string.IsNullOrEmpty(connectionString))
{
    // You might want more robust error handling or logging here
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
}
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
