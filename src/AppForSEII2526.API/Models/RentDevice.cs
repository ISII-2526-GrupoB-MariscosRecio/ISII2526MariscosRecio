Public class RentDevice
{


    [ForeignKey(nameof(DeviceId))]
    public Device Device { get; set; }

    public int DeviceId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "El alquiler a de tener un precio")]
    public double Price { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Porfa tienes que introducir una cantidad")]
    public int Quantity { get; set; }

    public int RentId { get; set; }

}