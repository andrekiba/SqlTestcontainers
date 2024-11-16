using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using SqlTestcontainers.Api.Controllers;
using SqlTestcontainers.DB;

namespace SqlTestcontainers.IntegrationTests1.Controllers.BikeController;

[TestClass]
public sealed class ReviewBikeTests
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
    public async Task When_BikeReview_Does_Not_Exist_In_Database_Then_Adds_It()
    {
        var expectedReview = fixture.Create<BikeReview>();

        await client.PutAsJsonAsync("bike/review", new ReviewBikeRequest(
            expectedReview.BikeId,
            expectedReview.UserId,
            expectedReview.ReviewContent
        ));

        var allBikeReviews = dbContext.BikeReviews.ToList();
        allBikeReviews.Should().ContainSingle().Which
            .Should().BeEquivalentTo(expectedReview, 
                options => options.Excluding(x => x.Id));
    }
    
    [TestMethod]
    public async Task When_BikeReview_Exists_In_Database_Already_Then_Updates_Review_Content()
    {
        var existingReview = fixture.Create<BikeReview>();
        await dbContext.AddAsync(existingReview);
        await dbContext.SaveChangesAsync();

        existingReview.ReviewContent = "This bike is the best one!";

        await client.PutAsJsonAsync("bike/review", new ReviewBikeRequest(
            existingReview.BikeId,
            existingReview.UserId,
            existingReview.ReviewContent
        ));

        var allBikeReviews = dbContext.BikeReviews.ToList();
        allBikeReviews.Count.Should().Be(1);
        allBikeReviews.Should().ContainSingle().Which
            .Should().BeEquivalentTo(existingReview,
                options => options.Excluding(x => x.Id));
    }
}