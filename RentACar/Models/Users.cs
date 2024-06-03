using System.ComponentModel.DataAnnotations;

namespace RentACar.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        public string? Name { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Surname cannot be longer than 50 characters.")]
        public string? Surname { get; set; }

        [Required(ErrorMessage = "The password field is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string? Password { get; set; }

        [Required]
        [Phone]
        [StringLength(20, ErrorMessage = "Phone number cannot be longer than 20 characters.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "The email field is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        public string? Email { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Address cannot be longer than 200 characters.")]
        public string? Address { get; set; }

        public string? UserType { get; set; } = "Standart";
    }
}
