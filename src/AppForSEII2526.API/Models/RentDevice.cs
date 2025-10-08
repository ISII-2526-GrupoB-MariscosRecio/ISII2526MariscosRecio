using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII2526.API.Models
{
    [PrimaryKey(nameof(DeviceId),nameof(RentId))] //FK nombradas de esta forma para no añadir codigo adicional al DB.context
    public class RentDevice
    {
        public Device Device { get; set; }

        public int DeviceId { get; set; }

        public Rental Rental { get; set; }
        public int RentId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "El alquiler a de tener un precio")]
        public double Price { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Porfa tienes que introducir una cantidad")]
        public int Quantity { get; set; }
    }
}