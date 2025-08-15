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
    [Display(Name = "Dator")]
    public Computer? Computer { get; set; } // Navigering till datorn

    [Required]
    [Display(Name = "Starttid")]
    public DateTime StartTime { get; set; } // Starttid (krävs)

    [Required]
    [Display(Name = "Sluttid")]
    public DateTime EndTime { get; set; } // Sluttid (krävs; ska vara > StartTime)

    [Required]
    public string? UserId { get; set; } // FK till IdentityUser

    [ForeignKey("UserId")]
    public IdentityUser? User { get; set; } // Navigering till användaren
}
