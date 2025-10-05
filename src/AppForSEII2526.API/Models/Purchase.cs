using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

public class Purchase
{
    [Key]
    public int Id { get; set; } // Primary key, necesaria (TODO: identificar claves foraneas)

    [Display(Name = "Nombre del cliente")] //No se como de grande es el nombre del cliente, pero me aseguro de que obligatoriamente utilize esta propiedad
    [Required(AllowEmptyStrings = false, ErrorMessage = "Atencion: es necesario que adjunte su nombre")]
    public string CustomerUserName { get; set; }
}