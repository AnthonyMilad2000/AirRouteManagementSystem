namespace AirRouteManagementSystem.Model
{
    public class Airport
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public string? Image { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public  string LocationLink { get; set; }
    }
}

