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

        
        public string Name { get; set; }//asumo que el nombre es imprescindible pero no le voy a poner una longitud definida

        public PaymentMethodTypes PaymentMethod { get; set; } //hare una enumeracion con los metodos de pago disponibles

       
        public DateTime RentalDate { get; set; }

        public DateTime RentalDateFrom { get; set; }

       
        public DateTime RentalDateTo { get; set; }

        public string Surname { get; set; } //No vamos a exigir que el apellido sea obligatorio

       
        public double TotalPrice { get; set; }

    }