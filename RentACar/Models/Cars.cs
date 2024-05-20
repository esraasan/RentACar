using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace RentACar.Models
{
    public class Cars
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? CarName { get; set; }

        [Required(ErrorMessage ="Brand name not null!")]
        [MaxLength(50)]
        [DisplayName("Brand Name")]
        public string? CarBrandName {  get; set; }

        [Required]
        [Range(0,250000)]
        public double CarKm { get; set; }
        [Required]
        public string? CarFuel {  get; set; }
        [Required]
        [Range(1000,20000)]
        public double CarPrice {  get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [ValidateNever]
        public string? ImgUrl {  get; set; }

    }
}
