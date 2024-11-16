using Microsoft.EntityFrameworkCore;
using SqlTestcontainers.DB;

namespace SqlTestcontainers.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        
        builder.Services.AddScoped<IBikeService, BikeService>();
        builder.Services.AddDbContext<BikeStoreContext>(options => options.UseSqlServer("Server=localhost;Database=BikeStore;User=sa;Password=Password123;"));

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}