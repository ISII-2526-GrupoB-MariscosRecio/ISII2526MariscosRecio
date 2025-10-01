using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models
{
    public class Rental
    {
        [Key]
        public int Id { get; set; }

        [DataType(System.Component.DataAnnotations.DataType.MultilineText)]
        [Display(Name="Dirreccion de entrega")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Porfa tienes que introducir tu direccion")]
        public string DeliveryAddress { get; set; }

        public string Name { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public DateTime RentalDate { get; set; }

        public DateTime RentalDateFrom { get; set; }

        public DateTime RentalDateTo { get; set; }

        public string Surname { get; set; }

        public double TotalPrice { get; set; }

    }









}
