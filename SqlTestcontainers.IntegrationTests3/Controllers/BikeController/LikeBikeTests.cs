using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using SqlTestcontainers.Api.Controllers;
using SqlTestcontainers.DB;

namespace SqlTestcontainers.IntegrationTests3.Controllers.BikeController;

[Collection(nameof(DatabaseTestCollection2))]
public class LikeBikeTests(IntegrationTestFactory factory) : DbTest(factory)
{
    [Fact]
    public async Task When_BikeLike_Does_Not_Exist_In_Database_Then_Adds_It()
    {
        var existingLike = Fixture.Create<BikeLike>();
        
        await Client.PutAsJsonAsync("bike/like", new LikeBikeRequest
        {
            BikeId = existingLike.BikeId,
            UserId = existingLike.UserId
        });

        var allBikeLikes = DbContext.BikeLikes.ToList();
        allBikeLikes.Should().ContainSingle().Which
            .Should().BeEquivalentTo(existingLike, 
                options => options.Excluding(x => x.Id));
    }
    
    [Fact]
    public async Task When_BikeLike_Exists_In_Database_Already_Then_Does_Nothing()
    {
        var existingLike = Fixture.Create<BikeLike>();
        await DbContext.AddAsync(existingLike);
        await DbContext.SaveChangesAsync();

        await Client.PutAsJsonAsync("bike/like", new LikeBikeRequest
        {
            BikeId = existingLike.BikeId,
            UserId = existingLike.UserId
        });

        var allBookLikes = DbContext.BikeLikes.ToList();
        allBookLikes.Should().ContainSingle();
    }
}