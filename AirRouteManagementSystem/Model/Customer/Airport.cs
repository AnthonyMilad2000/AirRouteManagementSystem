using System.ComponentModel.DataAnnotations;

namespace AirRouteManagementSystem.Model.Customer
{
    public class Airport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Range(-180, 180)]
        public double Longitude { get; set; }
    }
}
