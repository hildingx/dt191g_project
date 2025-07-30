using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

public class Booking
{
    public int Id { get; set; }

    [Required]
    public int ComputerId { get; set; }

    [ForeignKey("ComputerId")]
    public required Computer Computer { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    // Koppling till Identity-användare
    public required string UserId { get; set; }

    [ForeignKey("UserId")]
    public required IdentityUser User { get; set; }
}
