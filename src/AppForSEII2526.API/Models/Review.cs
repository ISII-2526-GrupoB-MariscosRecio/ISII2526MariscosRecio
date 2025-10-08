﻿using System;

namespace AppForSEII2526.API.Models;

public class Review
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "El pasi es un campo obligatorio")]
    public int CustomerCountry { get; set; }//Pais del cliente que hace la review, lo pongo como int para hacer una enumeracion


    public string? CustomerId { get; set; }//Identificador, nombre del cliente, opcional


    public date DateOfReview { get; set; }//Fecha de la reseña

    [Range(1,5, ErrorMessage = "La calificación tiene que estar entre 1 y 5")]
    [Required]
    public int OverallRating { get; set; }//Calificacion general del cliente, de 1 a 5

    [Key]
    public int ReviewId { get; set; }//Identificador de la reseña único

    [Required(AllowEmptyStrings = false, ErrorMessage = "El titulo de la reseña es obligatorio")]
    public string ReviewTitle { get; set; }//Titulo de la reseña obligatorio


    public IList<ReviewItem> ReviewItems { get; set; }

    public ApplicationUser ApplicationUser { get; set; }

}
