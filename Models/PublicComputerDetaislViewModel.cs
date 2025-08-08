public class PublicComputerDetailsViewModel
{
    public int ComputerId { get; set; }
    public string? Name { get; set; }
    public string? Location { get; set; }
    public bool IsAvailableNow { get; set; }
    public List<BookingSlotVM> Slots { get; set; } = new();
}

public class BookingSlotVM
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
