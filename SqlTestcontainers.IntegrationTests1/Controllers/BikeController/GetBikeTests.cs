using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using SqlTestcontainers.DB;

namespace SqlTestcontainers.IntegrationTests1.Controllers.BikeController;

[TestClass]
public sealed class GetBikeTests
{
    readonly IntegrationTestFactory factory = new();
    readonly Fixture fixture = new();
    BikeStoreContext dbContext = null!;
    HttpClient client = null!;
    
    [TestInitialize]
    public async Task TestInitialize()
    {
        await factory.InitializeAsync();
        dbContext = factory.Db;
        client = factory.CreateClient();
    }

    [TestCleanup]
    public async Task TestCleanup()
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