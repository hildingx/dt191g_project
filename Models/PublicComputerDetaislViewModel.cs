// Visningsmodell för publika detaljsidan för en dator.
public class PublicComputerDetailsViewModel
{
    public int ComputerId { get; set; }
    public string? Name { get; set; }
    public string? Location { get; set; }
    public bool IsAvailable { get; set; } // Aktiv i systemet
    public bool IsAvailableNow { get; set; } // Ledig vid "now"
    public List<BookingSlotVM> Slots { get; set; } = new(); // Kommande bokningar
}

// Tidsintervall för en bokning i listor/tabeller.
public class BookingSlotVM
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime   { get; set; }
    public string? UserEmail  { get; set; } // Visas endast för admin
}
