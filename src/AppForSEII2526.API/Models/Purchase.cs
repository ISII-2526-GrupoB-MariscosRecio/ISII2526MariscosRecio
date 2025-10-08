using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

public class Purchase
{
    [Key]
    public int Id { get; set; } // Primary key, necesaria (TODO: identificar claves foraneas)

    [Display(Name = "Nombre del cliente")] //No se como de grande es el nombre del cliente, pero me aseguro de que obligatoriamente utilize esta propiedad
    [Required(AllowEmptyStrings = false, ErrorMessage = "Atencion: es necesario que adjunte su nombre")]
    public string CustomerUserName { get; set; }

    [Display(Name = "Apellidos del cliente")] //Mismo caso para el nombre del cliente
    [Required(AllowEmptyStrings = false, ErrorMessage = "Atencion: es necesario que adjunte sus apellidos")]
    public string CustomerUserSurname { get; set; }

    [DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)] //Es posible que nuestro cliente no se limite a una l�nea para la direcci�n de la entrega del dispositivo
    [Display(Name = "Direccion de envio")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Atencion: es necesario que adjunte su direccion de envio del paquete")]
    public string DeliveryAddress { get; set; }

    [Display(Name = "Metodo de pago")]
    public PaymentMethod PaymentMethod { get; set; } //No es un tipo convencional / normal, por lo que he tenido que crear una enumeraci�n

    [DataType(System.ComponentModel.DataAnnotations.DataType.DateTime)]
    [Display(Name = "Fecha de la compra")]
    public DateTime PurchaseDate { get; set; } //Muy obvio que si hago una compra, saber cual fuel la fecha en la que la realiz�, ADEM�S DE SU HORA ESPECIFICA, por eso  el uso de DateTime.

    [Display(Name = "Precio total")]
    public double TotalPrice { get; set; }

    [Display(Name = "Cantidad dispositivos comprados")]]
    public int Quantity { get; set; }

    //TotalPrice y Quantity son bastante obvios.//Relacion N:N con PurchaseItem

    //Relacion N:N con PurchaseItem
    public IList<PurchaseItem> PurchaseItems { get; set; }

}//De clase purchase

//En el flujo b�sico, el sistema mostrar� una lista de dispositivos, todos ellos con sus respectivos daots, pero sobre todo el m�todo de pago. Al no ser un tipo convencional, necesito crear una enumeraci�n
public enum PaymentMethod
{
    TarjetaCredito, PayPal, Efectivo //Aunque en el flujo b�sico solo mencionen tarjeta de credito y paypal, considero poco l�gico no tener en cuenta el pago con efectivo.
}