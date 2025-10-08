using Microsoft.AspNetCore.Identity;
    
namespace AppForSEII2526.API.Models;

[PrimaryKey(nameof(MovieId), nameof(PurchaseId))]
public class PurchaseItem
{
    
    public int PurchaseId { get; set; } // Parte de una foreing key.
    public Purchase Purchase { get; set; } //Parte de una foreing key N:N

    //Las descripciones no deben de ser tan largas, m�nimo 50 caracteres, m�ximo 500 caracteres
    [Display(Name = "Descripcion")]
    public string? Description { get; set; }

    [Precision(6, 2)] //La precisi�n indica el total de d�gitos, y la cantidad de estos que se usaran para la parte decimal.
    public double Price { get; set; }

    public int DeviceId { get; set; } // Parte de Foreign key, preguntar en clase como se hace (peque�o cambio en como se nombra atributo)  
    public Device Device { get; set; } //Una parte de la Foreign key N:N

    public int Quantity { get; set; } //Cantidad de dispositivos comprados de este tipo

    //PurchaseID y DeviceID forman la clave foranea compuesta de esta tabla intermedia. Se han declarado listas en Purchase y Device

}//De clase PurchaseItem