using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models
{
    public class Rental
    {
        [Key]
        public int Id { get; set; } //hecho

        [DataType(System.Component.DataAnnotations.DataType.MultilineText)]
        [Display(Name = "Dirreccion de entrega")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Porfa tienes que introducir tu direccion")]
        public string DeliveryAddress { get; set; }//hecho

        [Display(Name = "Nombre")]
        [Required(AlowEmptyStrings = false, ErrorMessage = "Porfa introduce tu nombre")]
        public string Name { get; set; }//asumo que el nombre es imprescindible pero no le voy a poner una longitud definida

        [Display(Name = "Metodos de pago")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Porfa tienes que introducir metodo de pago")]
        public PaymentMethodTypes PaymentMethod { get; set; } //hare una enumeracion con los metodos de pago disponibles

        [DataType(DataType.Date)]
        [Display(Name = "Fecha del alquiler")]
        public DateTime? RentalDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de comienzo de alquiler")]
        public DateTime? RentalDateFrom { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de fin del alquiler")]
        public DateTime? RentalDateTo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Porfa tienes que introducir tu apellido")]
        public string Surname { get; set; } 

        [DataType(DataType.Currency)]
        [Display(Name = "Precio del alquiler")]
        public double? TotalPrice { get; set; }

        public IList<Rental> RentalItems { get; set; } //Segunda parte de la clave foranea con RentDevice


    }
    //Enumeracion para los metodos de pago
    public enum PaymentMethodTypes
    {
        Tarjeta, PayPal, Efectivo, Bizum
    }