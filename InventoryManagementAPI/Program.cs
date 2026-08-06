using InventoryManagementAPI.Data;
using Microsoft.EntityFrameworkCore;
using InventoryManagementAPI.Repositories;
using InventoryManagementAPI.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("db_imsAPI") 
        ?? throw new InvalidOperationException("Connection string 'AppDbContext' not found.")));

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddControllers(); 

builder.Services.AddOpenApi();

var app = builder.Build(); 


if (app.Environment.IsDevelopment()) 
{
    app.MapOpenApi();
}

app.UseHttpsRedirection(); 

app.UseAuthorization();

app.MapControllers(); 

app.Run(); 
