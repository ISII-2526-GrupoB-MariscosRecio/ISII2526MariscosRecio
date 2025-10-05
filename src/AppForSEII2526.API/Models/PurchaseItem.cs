using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

public class PurchaseItem
{
    [Key]
    public int PurchaseId { get; set; } // Primary key, necesaria (TODO: identificar claves foraneas)


}//De clase PurchaseItem