using System;

public class ReviewItem
{
    [Required(AllowEmptyString = false, ErrorMessage = "El comentario no puede estar vacio")]
    public string Comments { get; set; }//Comentario del usuario

    public int DeviceId { get; set; }//Identificador del dispositivo que se esta reseñando

    [Key]
    public int Id { get; set; }//Identificador unico

    [Required]
    public int Rating { get; set; }//Calificacion del dispositivo obligatoria


    public int ReviewId { get; set; }//Identificador de la reseña a la que pertenece el item




}
