using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlTestcontainers.DB;

public class BikeReview
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int BikeId { get; set; }
    public int UserId { get; set; }
    public string ReviewContent { get; set; } = null!;
}