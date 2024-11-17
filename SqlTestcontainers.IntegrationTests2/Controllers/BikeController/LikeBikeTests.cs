using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using SqlTestcontainers.Api.Controllers;
using SqlTestcontainers.DB;

namespace SqlTestcontainers.IntegrationTests2.Controllers.BikeController;

[TestClass]
public sealed class LikeBikeTests
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
    
    [TestCleanup]
    public async Task TestCleanup()
    {
        await factory.ResetDatabase();
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