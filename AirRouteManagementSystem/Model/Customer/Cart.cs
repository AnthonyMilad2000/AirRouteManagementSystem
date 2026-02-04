using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirRouteManagementSystem.Model.Customer
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int FlightId { get; set; }

        [ForeignKey(nameof(FlightId))]
        public Flight Flight { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}
