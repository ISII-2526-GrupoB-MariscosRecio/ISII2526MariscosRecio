using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
//he añadido nombre y apellido a la clase user
public class ApplicationUser : IdentityUser {
    [Display(Name = "Name")]
    public string Name
    {
        get;
        set;
    }

    [Display(Name = "Surname")]
    public string Surname
    {
        get;
        set;
    }
}