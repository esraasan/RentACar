// URentController.cs

namespace RentACar.Controllers
{
    internal class CarModel
    {
        public int Id { get; set; }
        public string CarName { get; set; }
        public string ImgUrl { get; set; }
        public string CarBrandName { get; set; }
        public double CarKm { get; set; }
        public string CarFuel { get; set; }
        public double CarPrice { get; set; }
        public int UserId { get; internal set; }
        public DateTime StartDate { get; internal set; }
        public DateTime EndDate { get; internal set; }
    }
}