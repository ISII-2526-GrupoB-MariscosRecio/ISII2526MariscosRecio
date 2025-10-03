using System;

public class Review
{
    
    [Required(AllowEmptyStrings = false, ErrorMessage = "El pais es un campo obligatorio")]
    public int CustomerCountry { get; set; }//Pais del cliente


    public int CustomerId { get; set; }//Identificador del cliente

    public DateTime DateOfReview { get; set; }//Fecha de la reseña

    [Range(1,5, ErrorMessage = "La calificacion tiene que estar entre 1 y 5")]
    [Required]
    public int OverallRating { get; set; }//Calificacion que le da el cliente al producto

    [Key]
    public int ReviewId { get; set; }//Identificador de la reseña único

    [Required(AllowEmptyStrings = false, ErrorMessage = "Titulo obligatorio para la reseña")]
    public string ReviewTitle { get; set; }//Titulo de la reseña


}
