using AutoFixture;
using SqlTestcontainers.DB;

namespace SqlTestcontainers.IntegrationTests3;

public class DbTest: IAsyncLifetime//, IClassFixture<IntegrationTestFactory>
{
    protected readonly Fixture Fixture = new();
    protected readonly BikeStoreContext DbContext;
    protected readonly HttpClient Client;
    readonly Func<Task> resetDatabase;
    
    protected DbTest(IntegrationTestFactory factory)
    {
        resetDatabase = factory.ResetDatabase;
        DbContext = factory.Db;
        Client = factory.CreateClient();
    }
    
    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        DbContext.ChangeTracker.Clear();
        return resetDatabase();
    }
    
    protected async Task Insert<T>(T entity) where T : class
    {
        await DbContext.AddAsync(entity);
        await DbContext.SaveChangesAsync();
    }
    
    protected async Task InsertMany<T>(IEnumerable<T> entities) where T : class
    {
        await DbContext.AddRangeAsync(entities);
        await DbContext.SaveChangesAsync();
    }
}