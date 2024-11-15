using Microsoft.AspNetCore.Mvc;
using SqlTestcontainers.DB;

namespace SqlTestcontainers.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BikeController(IBikeService bikeService)
{
    [HttpGet("{name}")]
    public async Task<Bike> GetBike([FromRoute] string name)
    {
        return await bikeService.GetBike(name);
    }

    [HttpPut("like")]
    public async Task LikeBike([FromBody] LikeBikeRequest request)
    {
        await bikeService.LikeBike(request.BikeId, request.UserId);
    }

    [HttpPut("review")]
    public async Task ReviewBike([FromBody] ReviewBikeRequest request)
    {
        await bikeService.ReviewBike(request.BikeId, request.UserId, request.ReviewContent);
    }
}

public class LikeBikeRequest
{
    public int BikeId { get; set; }
    public int UserId { get; set; }
}

public record ReviewBikeRequest(int BikeId, int UserId, string ReviewContent);