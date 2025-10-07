using System;

namespace AppForSEII2526.API.Models
[PrimaryKey(nameof(DeviceId), nameof(ReviewId)]
public class ReviewItem
{
    [Required(AllowEmptyString = false, ErrorMessage = "El comentario no puede estar vacío")]
    public string Comments { get; set; }//Comentario del review item

    
    public Device Device { get; set; }//Identificador del dispositivo al que se le hace el review

    
    public int Id { get; set; }//Identificador único del review item


    [Range(1,5, ErrorMessage = "La calificación tiene que estar entre 1 y 5")]
    [Required]
    public int Rating { get; set; }//Calificación del dispositivo


    public Review Review { get; set; }//Identificador de la review a la que pertenece el review item

}
