using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

public class Purchase
{
    [Key]
    public int Id { get; set; } // Primary key, necesaria

    [Display(Name = "Usuario del cliente")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Atencion: es necesario que adjunte algun usuario")]
    public string CustomerUserName { get; set; } 

}//De clase purchase