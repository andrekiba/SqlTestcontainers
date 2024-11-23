using AutoFixture;
using FluentAssertions;
using SqlTestcontainers.Api;
using SqlTestcontainers.DB;

namespace SqlTestcontainers.IntegrationTests3.Controllers.BikeController;

[Collection(nameof(DatabaseTestCollection4))]
public class ManyReviewBikeTests : DbTest, IAsyncLifetime
{
    readonly BikeService bikeService;
    
    public ManyReviewBikeTests(IntegrationTestFactory factory) : base(factory)
    {
        bikeService = new BikeService(DbContext);   
    }
    
    public new async Task InitializeAsync()
    {
        var bikes = Fixture.CreateMany<Bike>(100);
        await InsertMany(bikes);
        await base.InitializeAsync();
    }
    
    [Theory, Repeat(100)]
    public async Task When_BikeReview_Does_Not_Exist_In_Database_Then_Adds_It(int iteration)
    {
        var expectedReview = Fixture.Create<BikeReview>();
        expectedReview.BikeId = iteration;

        await bikeService.ReviewBike(expectedReview.BikeId, expectedReview.UserId, expectedReview.ReviewContent);
        
        var allBikeReviews = DbContext.BikeReviews.ToList();
        allBikeReviews.Should().ContainSingle().Which
            .Should().BeEquivalentTo(expectedReview, 
                options => options.Excluding(x => x.Id));
    }
    
    [Theory, Repeat(50)]
    public async Task When_BikeReview_Exists_In_Database_Already_Then_Updates_Review_Content(int iteration)
    {
        var existingReview = Fixture.Create<BikeReview>();
        existingReview.Id = iteration;
        await Insert(existingReview);

        existingReview.ReviewContent = "This bike is the best one!";

        await bikeService.ReviewBike(existingReview.BikeId, existingReview.UserId, existingReview.ReviewContent);

        var allBikeReviews = DbContext.BikeReviews.ToList();
        allBikeReviews.Count.Should().Be(1);
        allBikeReviews.Should().ContainSingle().Which
            .Should().BeEquivalentTo(existingReview,
                options => options.Excluding(x => x.Id));
    }
}