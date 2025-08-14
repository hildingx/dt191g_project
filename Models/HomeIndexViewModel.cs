// Visningsmodell för startsidan: utvalda lediga datorer.
public class HomeIndexViewModel
{
    public List<ComputerAvailabilityViewModel> FeaturedComputers { get; set; } = new(); // Max 3 i controller
}