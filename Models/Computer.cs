using System.ComponentModel.DataAnnotations;

public class Computer
{
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Location { get; set; }

    public bool IsAvailable { get; set; }

    // Navigation
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
