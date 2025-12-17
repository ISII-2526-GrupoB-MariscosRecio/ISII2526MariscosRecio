using AppForSEII2526.Web.API;



namespace AppForSEII2526.Web {
    public class RentalStateContainer {
        // El objeto principal que guardará los datos del alquiler
        public RentalPostDTO Rental { get; private set; } = new RentalPostDTO() {
            RentalItems = new List<RentalItemDTO>()
        };

        // Calculamos el precio total dinámico basándonos en los días y la cantidad
        public decimal TotalPrice {
            get {
                int numberOfDays = (Rental.RentalDateTo - Rental.RentalDateFrom).Days;
                return Convert.ToDecimal(Rental.RentalItems.Sum(ri => ri.RentPrice * numberOfDays));
            }
        }

        // Evento para avisar a los componentes que algo ha cambiado
        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

        // MÉTODO CLAVE: Añadir dispositivo al carrito
        public void AddDeviceToRental(DeviceParaRentDTO device) {
            //antes de añadir el dispositivo compruebo que no haya sido añadido antes
            if (!Rental.RentalItems.Any(ri => ri.Id == device.Id))
                //lo añado a la lista a mi "CARRITO DE LA COMPRA"
                Rental.RentalItems.Add(new RentalItemDTO() {
                    Id = device.Id,
                    Brand = device.Brand,
                    Model = device.Model,
                    RentPrice= device.RentPrice,
                    Quantity = 1
                }
            );

        }

        // Eliminar un ítem del carrito
        public void RemoveRentalItemToRent(RentalItemDTO item) {
            Rental.RentalItems.Remove(item);
        
        }

        // Vaciar el carrito (por ejemplo, al cambiar fechas)
        public void ClearRentingCart() {
            Rental.RentalItems.Clear();
            
        }

        // Reiniciar todo después de confirmar el alquiler
        public void RentalProcessed() {
            Rental = new RentalPostDTO() {
                RentalItems = new List<RentalItemDTO>()
            };
           
        }
    }
}