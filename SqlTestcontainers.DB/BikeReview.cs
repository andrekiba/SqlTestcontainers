using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlTestcontainers.DB;

public class BikeReview
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public int BikeId { get; set; }
    public int UserId { get; set; }
    public string ReviewContent { get; set; } = null!;
}