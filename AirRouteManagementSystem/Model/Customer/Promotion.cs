using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirRouteManagementSystem.Model.Customer
{
    public class Promotion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string PromotionCode { get; set; }

        [Required]
        [MaxLength(20)]
        public string DiscountType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountValue { get; set; }

        [Required]
        public int MaxUse { get; set; }

        public int UsedCount { get; set; }

        [Required]
        public DateTime ExpireDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; }
    }
}
