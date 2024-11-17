using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using SqlTestcontainers.DB;

namespace SqlTestcontainers.IntegrationTests2.Controllers.BikeController;

[TestClass]
public sealed class GetBikeTests
{
    static readonly IntegrationTestFactory factory = new();
    readonly Fixture fixture = new();
    static BikeStoreContext dbContext = null!;
    static HttpClient client = null!;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext testContext)
    {
        await factory.InitializeAsync();
        dbContext = factory.Db;
        client = factory.CreateClient();
    }

    [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
    public static async Task ClassCleanup()
    {
        await factory.DisposeAsync();
    }
    
    [TestMethod]
    public async Task When_Bike_Exists_Then_Returns_It()
    {
        var existingBike = fixture.Create<Bike>();
        await dbContext.AddAsync(existingBike);
        await dbContext.SaveChangesAsync();
        
        var book = await client.GetFromJsonAsync<Bike>($"bike/{existingBike.Name}");

        book.Should().NotBeNull();
    }
}