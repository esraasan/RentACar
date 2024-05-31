using System.ComponentModel.DataAnnotations;

namespace RentACar.Models
{
    public class Rental
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CarId { get; set; }
        public Cars? Car { get; set; }
        public string CarName { get; set; }
        public string CarBrandName { get; set; }

        [Required]
        public int UserId { get; set; }
        public Users? User { get; set; }

        public DateTime StartDate { get; set; }       
        public DateTime EndDate { get; set; }
        public double CarPrice { get; set; }

        public void CalculateRentalPrice(double carPrice)
        {
            int rentDurationInDays = (int)(EndDate.Date - StartDate.Date).TotalDays;
            CarPrice = rentDurationInDays * carPrice;
        }

    }
}
