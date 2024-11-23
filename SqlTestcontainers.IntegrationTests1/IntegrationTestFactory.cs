using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SqlTestcontainers.Api;
using SqlTestcontainers.DB;
using Testcontainers.MsSql;

namespace SqlTestcontainers.IntegrationTests1;

public class IntegrationTestFactory : WebApplicationFactory<Program>
{
    readonly MsSqlContainer container = new MsSqlBuilder()
        //.WithImage("...")
        //.WithVolumeMount("source", "target")
        .Build();

    public BikeStoreContext Db { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        await container.StartAsync();
        Db = Services.CreateScope().ServiceProvider.GetRequiredService<BikeStoreContext>();
    }

    public new async Task DisposeAsync()
    {
        await container.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveDbContext<BikeStoreContext>();
            services.AddDbContext<BikeStoreContext>(options =>
            {
                options.UseSqlServer(container.GetConnectionString());
            });
            services.EnsureDbCreated<BikeStoreContext>();
        });
    }
}

public static class ServiceCollectionExtensions
{
    public static void RemoveDbContext<T>(this IServiceCollection services) where T : DbContext
    {
        var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<T>));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
    }

    public static void EnsureDbCreated<T>(this IServiceCollection services) where T : DbContext
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var context = serviceProvider.GetRequiredService<T>();
        context.Database.EnsureCreated();
    }
}