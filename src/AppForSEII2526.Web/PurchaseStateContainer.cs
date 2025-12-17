using AppForSEII2526.Web.API; //CAMBIO IMPORTANTE: solo usamos las clases que genera Swagger

namespace AppForSEII2526.Web
{
    public class PurchaseStateContainer
    {
        // Propiedad que almacenará los datos de la compra a enviar
        public PurchaseForCreateDTO Purchase { get; private set; }

        // Propiedad auxiliar para mostrar el precio total en la UI (ya que el DTO no lo guarda explícitamente)
        public decimal TotalPrice { get; private set; }

        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

        public PurchaseStateContainer()
        {
            // Inicializamos el DTO. Como tu DTO tiene constructor con parámetros, debemos enviarlos.
            // Inicializamos listas vacías y strings vacíos por defecto.
            ResetPurchase();
        }

        public void AddDeviceToPurchase(PurchaseItemDTO device)
        {
            //Prueba de seguridad para comprobar que la lista se ha inicializado (seguridad con el cliente generado)
            if (Purchase.PurchaseItems == null)
            {
                Purchase.PurchaseItems = new List<PurchaseItemDTO>();
            }

            // Comprobamos si el dispositivo ya está en el carrito
            var existingItem = Purchase.PurchaseItems.FirstOrDefault(pi => pi.Id == device.Id);

            if (existingItem != null)
            {
                // Si ya existe, simplemente aumentamos la cantidad (asumiendo que quieres sumar 1)
                existingItem.Quantity += device.Quantity > 0 ? device.Quantity : 1;
            }
            else
            {
               
                // Si no existe, lo añadimos a la lista
                // cambio: usamos inicialización de objeto ({}) para mayor claridad
                Purchase.PurchaseItems.Add(new PurchaseItemDTO
                {
                    Id = device.Id,
                    Brand = device.Brand,
                    Model = device.Model,
                    Color = device.Color,
                    UnitPrice = device.UnitPrice,
                    Quantity = device.Quantity > 0 ? device.Quantity : 1,  // Por defecto 1 si viene a 0
                    Description = device.Description ?? string.Empty
                });
            }

            ComputeTotalPrice();
            NotifyStateChanged();
        }

        public void RemoveDeviceFromPurchase(PurchaseItemDTO item)
        {
            Purchase.PurchaseItems.Remove(item);
            ComputeTotalPrice();
            NotifyStateChanged();
        }

        public void ClearPurchaseCart()
        {
            Purchase.PurchaseItems.Clear();
            TotalPrice = 0;
            NotifyStateChanged();
        }

        // Método para recalcular el precio total basado en items y cantidades
        private void ComputeTotalPrice()
        {
            if (Purchase.PurchaseItems != null) // Simple verificación de seguridad para que no pete el cliente generado
            {
                // double, decimal e int, son tipos compatibles en operaciones aritméticas
                // cliente generado suele usar 'double' para precios. 
                TotalPrice = (decimal)Purchase.PurchaseItems.Sum(pi => pi.UnitPrice * pi.Quantity);
            }
        }

        // Se llama cuando se completa la compra (POST exitoso) para reiniciar el estado
        public void PurchaseProcessed()
        {
            ResetPurchase();
            NotifyStateChanged();
        }

        // Método auxiliar para reiniciar/inicializar el objeto
        private void ResetPurchase()
        {
            Purchase = new PurchaseForCreateDTO { 
                CustomerUserName = string.Empty,                // CustomerUserName
                CustomerUserSurname = string.Empty,             // CustomerUserSurname
                DeliveryAddress = string.Empty,                 // DeliveryAddress
                PaymentMethod = PaymentMethod.TarjetaCredito,   // Valor por defecto (ajusta según tu Enum)
                PurchaseItems = new List<PurchaseItemDTO>()
            };
            TotalPrice = 0;
        }
    }
}
