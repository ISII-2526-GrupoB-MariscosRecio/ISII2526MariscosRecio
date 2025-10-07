using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

public class PurchaseItem
{
    
    public int PurchaseId { get; set; } // Parte de una foreing key.

    //Las descripciones no deben de ser tan largas, m�nimo 50 caracteres, m�ximo 500 caracteres
    [Display(Name = "Descripcion")]
    [Required(500, ErrorMessage = "Atencion: la descripcion no puede ser m�s larga de 500 caracteres", MinimumLength = 50)]
    public string Description { get; set; }

    [Precision(6, 2)] //La precisi�n indica el total de d�gitos, y la cantidad de estos que se usaran para la parte decimal.
    public double Price { get; set; }

    public int DeviceID { get; set; } // Parte de Foreign key, preguntar en clase como se hace

    public int Quantity { get; set; } //Cantidad de dispositivos comprados de este tipo

}//De clase PurchaseItem