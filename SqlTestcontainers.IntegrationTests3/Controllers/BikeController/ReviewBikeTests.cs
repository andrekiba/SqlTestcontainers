using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using SqlTestcontainers.Api.Controllers;
using SqlTestcontainers.DB;

namespace SqlTestcontainers.IntegrationTests3.Controllers.BikeController;

[Collection(nameof(DatabaseTestCollection3))]
public class ReviewBikeTests(IntegrationTestFactory factory) : DbTest(factory)
{
    [Fact]
    public async Task When_BikeReview_Does_Not_Exist_In_Database_Then_Adds_It()
    {
        var expectedReview = Fixture.Create<BikeReview>();

        await Client.PutAsJsonAsync("bike/review", new ReviewBikeRequest(
            expectedReview.BikeId,
            expectedReview.UserId,
            expectedReview.ReviewContent
        ));

        var allBikeReviews = DbContext.BikeReviews.ToList();
        allBikeReviews.Should().ContainSingle().Which
            .Should().BeEquivalentTo(expectedReview, 
                options => options.Excluding(x => x.Id));
    }
    
    [Fact]
    public async Task When_BikeReview_Exists_In_Database_Already_Then_Updates_Review_Content()
    {
        var existingReview = Fixture.Create<BikeReview>();
        await DbContext.AddAsync(existingReview);
        await DbContext.SaveChangesAsync();

        existingReview.ReviewContent = "This bike is the best one!";

        await Client.PutAsJsonAsync("bike/review", new ReviewBikeRequest(
            existingReview.BikeId,
            existingReview.UserId,
            existingReview.ReviewContent
        ));

        var allBikeReviews = DbContext.BikeReviews.ToList();
        allBikeReviews.Count.Should().Be(1);
        allBikeReviews.Should().ContainSingle().Which
            .Should().BeEquivalentTo(existingReview,
                options => options.Excluding(x => x.Id));
    }
}