using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

public class Purchase
{
    [Key]
    public int Id { get; set; } // Primary key, necesaria

    [Display(Name = "Nombre del cliente")] //No se como de grande es el nombre del cliente, pero me aseguro de que obligatoriamente utilize esta propiedad
    [Required(AllowEmptyStrings = false, ErrorMessage = "Atencion: es necesario que adjunte su nombre")] 
    public string CustomerUserName { get; set; }

    [Display(Name = "Apellidos del cliente")] //Mismo caso para el nombre del cliente
    [Required(AllowEmptyStrings = false, ErrorMessage = "Atencion: es necesario que adjunte sus apellidos")]
    public string CustomerUserSurname { get; set; }

    [DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)] //Es posible que nuestro cliente no se limite a una línea para la dirección de la entrega del dispositivo
    [Display(Name = "Direccion de envio")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Atencion: es necesario que adjunte su direccion de envio del paquete")]
    public string DeliveryAddress { get; set; }

}//De clase purchase