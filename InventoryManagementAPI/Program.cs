using InventoryManagementAPI.Data;
using Microsoft.EntityFrameworkCore;
using InventoryManagementAPI.Repositories;
using InventoryManagementAPI.Repositories.Interfaces;
using InventoryManagementAPI.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("db_imsAPI") 
        ?? throw new InvalidOperationException("Connection string 'AppDbContext' not found.")));

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment()) 
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); 

app.UseAuthorization();

app.MapControllers(); 

app.Run(); 
