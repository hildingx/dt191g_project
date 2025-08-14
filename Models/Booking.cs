using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

// Representerar en bokning av en specifik dator under ett tidsintervall.
public class Booking
{
    public int Id { get; set; } // Primärnyckel

    [Required]
    public int ComputerId { get; set; } // FK till Computer

    [ForeignKey("ComputerId")]
    public Computer? Computer { get; set; } // Navigering till datorn

    [Required]
    public DateTime StartTime { get; set; } // Starttid (krävs)

    [Required]
    public DateTime EndTime { get; set; } // Sluttid (krävs; ska vara > StartTime)

    [Required]
    public string? UserId { get; set; } // FK till IdentityUser

    [ForeignKey("UserId")]
    public IdentityUser? User { get; set; } // Navigering till användaren
}
