public class ComputerAvailabilityViewModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Location { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsAvailableNow { get; set; }
    
}
