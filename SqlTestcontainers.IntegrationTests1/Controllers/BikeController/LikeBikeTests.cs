using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using SqlTestcontainers.Api.Controllers;
using SqlTestcontainers.DB;

namespace SqlTestcontainers.IntegrationTests1.Controllers.BikeController;

[TestClass]
public sealed class LikeBikeTests
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
    public async Task When_BikeLike_Does_Not_Exist_In_Database_Then_Adds_It()
    {
        var existingLike = fixture.Create<BikeLike>();
        
        await client.PutAsJsonAsync("bike/like", new LikeBikeRequest
        {
            BikeId = existingLike.BikeId,
            UserId = existingLike.UserId
        });

        var allBikeLikes = dbContext.BikeLikes.ToList();
        allBikeLikes.Should().ContainSingle().Which
            .Should().BeEquivalentTo(existingLike, 
                options => options.Excluding(x => x.Id));
    }
    
    [TestMethod]
    public async Task When_BikeLike_Exists_In_Database_Already_Then_Does_Nothing()
    {
        var existingLike = fixture.Create<BikeLike>();
        await dbContext.AddAsync(existingLike);
        await dbContext.SaveChangesAsync();

        await client.PutAsJsonAsync("bike/like", new LikeBikeRequest
        {
            BikeId = existingLike.BikeId,
            UserId = existingLike.UserId
        });

        var allBookLikes = dbContext.BikeLikes.ToList();
        allBookLikes.Should().ContainSingle();
    }
}