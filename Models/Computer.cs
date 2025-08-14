using System.ComponentModel.DataAnnotations;

// Representerar en bokningsbar dator/arbetsstation.
public class Computer
{
    public int Id { get; set; } // Primärnyckel

    [Required]
    public string? Name { get; set; } // Visningsnamn

    [Required]
    public string? Location { get; set; } // Plats/rum

    public bool IsAvailable { get; set; } // Aktiv/bokningsbar i systemet

    // Navigering: alla bokningar för datorn
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
