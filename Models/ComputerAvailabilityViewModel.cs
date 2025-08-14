// Visningsmodell för översikter: basinfo + realtidsstatus.
public class ComputerAvailabilityViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = ""; // Aldrig null i vyer
    public string Location { get; set; } = ""; // Aldrig null i vyer
    public bool IsAvailable { get; set; } // Aktiv i systemet
    public bool IsAvailableNow { get; set; } // Ledig vid "now" (ingen pågående bokning)
}
