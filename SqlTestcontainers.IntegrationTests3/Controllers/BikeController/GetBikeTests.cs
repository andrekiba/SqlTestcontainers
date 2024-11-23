using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using SqlTestcontainers.DB;

namespace SqlTestcontainers.IntegrationTests3.Controllers.BikeController;

[Collection(nameof(DatabaseTestCollection1))]
public class GetBikeTests(IntegrationTestFactory factory) : DbTest(factory)
{
    [Fact]
    public async Task When_Bike_Exists_Then_Returns_It()
    {
        var existingBike = Fixture.Create<Bike>();
        await Insert(existingBike);
        
        var book = await Client.GetFromJsonAsync<Bike>($"bike/{existingBike.Name}");

        book.Should().NotBeNull();
    }
}