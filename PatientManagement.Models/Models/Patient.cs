using System;
using System.ComponentModel.DataAnnotations;

namespace YourNamespace.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }

        [Required]
        [StringLength(15)]
        public string ContactNumber { get; set; }

        [Range(0, double.MaxValue)]
        public double Weight { get; set; }

        [Range(0, double.MaxValue)]
        public double Height { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(1000)]
        public string MedicalComments { get; set; }

        public bool AnyMedicationsTaking { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
