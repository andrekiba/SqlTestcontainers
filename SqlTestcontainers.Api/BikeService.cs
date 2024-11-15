using Microsoft.EntityFrameworkCore;
using SqlTestcontainers.DB;

namespace SqlTestcontainers.Api;

public interface IBikeService
{
    Task<Bike> GetBike(string name);
    Task LikeBike(int bikeId, int userId);
    Task ReviewBike(int bikeId, int userId, string reviewContent);
}

public class BikeService : IBikeService
{
    readonly BikeStoreContext context;
    
    public BikeService(BikeStoreContext context)
    {
        this.context = context;
    }

    public async Task<Bike> GetBike(string name)
    {
        return await context.Bikes.FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task LikeBike(int bikeId, int userId)
    {
        var existingBikeLike = await context.BikeLikes.FirstOrDefaultAsync(x => x.BikeId == bikeId && x.UserId == userId);

        if (existingBikeLike != null)
            return;
        
        await context.BikeLikes.AddAsync(new BikeLike
        {
            BikeId = bikeId,
            UserId = userId
        });

        await context.SaveChangesAsync();
    }

    public async Task ReviewBike(int bikeId, int userId, string reviewContent)
    {
        var existingBikeReview = await context.BikeReviews.FirstOrDefaultAsync(x => x.BikeId == bikeId && x.UserId == userId);

        if (existingBikeReview == null)
        {
            await context.BikeReviews.AddAsync(new BikeReview
            {
                BikeId = bikeId,
                UserId = userId,
                ReviewContent = reviewContent
            });
        }
        else
        {
            existingBikeReview.ReviewContent = reviewContent;
        }

        await context.SaveChangesAsync();
    }
}