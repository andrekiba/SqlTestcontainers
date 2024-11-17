using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using SqlTestcontainers.Api;
using SqlTestcontainers.DB;
using Testcontainers.MsSql;

namespace SqlTestcontainers.IntegrationTests3;

[CollectionDefinition(nameof(DatabaseTestCollection1))]
public class DatabaseTestCollection1 : ICollectionFixture<IntegrationTestFactory>;

[CollectionDefinition(nameof(DatabaseTestCollection2))]
public class DatabaseTestCollection2 : ICollectionFixture<IntegrationTestFactory>;

[CollectionDefinition(nameof(DatabaseTestCollection3))]
public class DatabaseTestCollection3 : ICollectionFixture<IntegrationTestFactory>;

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    readonly MsSqlContainer container = new MsSqlBuilder().Build();

    public BikeStoreContext Db { get; private set; } = default!;
    DbConnection connection = default!;
    Respawner respawner = default!;
    
    public async Task ResetDatabase()
    {
        await respawner.ResetAsync(connection);
    }

    public async Task InitializeAsync()
    {
        await container.StartAsync();
        Db = Services.CreateScope().ServiceProvider.GetRequiredService<BikeStoreContext>();
        connection = Db.Database.GetDbConnection();
        await connection.OpenAsync();

        respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            SchemasToInclude = ["dbo"]
        });
    }

    public new async Task DisposeAsync()
    {
        await connection.CloseAsync();
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