using System.ComponentModel.DataAnnotations;

public class Computer
{
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    public required string Location { get; set; }

    public bool IsAvailable { get; set; }

    // Navigation
    public required ICollection<Booking> Bookings { get; set; }
}
