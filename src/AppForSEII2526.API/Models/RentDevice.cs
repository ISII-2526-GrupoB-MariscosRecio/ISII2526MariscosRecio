using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models

[PrimaryKey(nameof(DeviceId),nameof(RentId)] //FK nombradas de esta forma para no añadir codigo adicional al DB.context
Public class RentDevice
{


    
    public Device Device { get; set; }

    public int DeviceId { get; set; }

    public Rental Rent { get; set; }

    public int RentId { get; set; }

    

    [Required(AllowEmptyStrings = false, ErrorMessage = "El alquiler a de tener un precio")]
    public double Price { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Porfa tienes que introducir una cantidad")]
    public int Quantity { get; set; }

    

}