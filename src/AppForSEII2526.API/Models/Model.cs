using System;

namespace AppForSEII2526.API.Models

public class Model
{
	
	[Key]
	public int id {  get; set; }//identificador único del modelo


	[StringLength(30,ErrorMessage="El nombre del modelo no puede tener mas de 30 caracteres")]
	public string NameModel { get; set; }//nombre del modelo 

	
}
