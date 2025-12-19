using System;
using System.Collections.Generic;
using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using AppForSEII2526.UIT.Shared;

namespace AppForSEII2526.UIT.Purchase
{
    public class UC_Purchase_UIT : UC_UIT, IDisposable
    {
        public UC_Purchase_UIT() : base()
        {
            // El driver se inicia en la clase base UC_UIT
        }

        [Fact(DisplayName = "Esc-1: Compra Correcta (Flujo Básico)")]
        public void Purchase_Flow_Basic_Success()
        {
            // 1. ARRANGE: Preparamos los objetos de página
            var selectPO = new SelectDevice_PO(_driver, null);
            var createPO = new CreatePurchase_PO(_driver, null);
            var detailsPO = new DetailsPurchase_PO(_driver, null);

            // Datos de prueba
            string deviceToBuy = "iPhone 13"; // Asegúrate de que este móvil existe en tu BD
            string userName = "Juan";
            string userSurname = "Perez";
            string address = "Calle Falsa 123";
            string payment = "CreditCard"; // Debe coincidir con uno de tu <select>

            // 2. ACT & ASSERT (Paso a Paso)

            // --- PASO 1: SELECCIÓN ---
            selectPO.Visit();
            selectPO.SelectDevice(deviceToBuy);
            selectPO.ClickTramitar();

            // --- PASO 2: CREACIÓN (FORMULARIO) ---
            // Verificamos que el ítem está en la tabla resumen antes de pagar
            var expectedItems = new List<string[]> {
                new string[] { deviceToBuy }
            };
            Assert.True(createPO.CheckListOfPurchaseItems(expectedItems), "El dispositivo seleccionado no aparece en el resumen del pedido.");

            // Rellenamos formulario y enviamos
            createPO.FillInPurchaseInfo(userName, userSurname, address, payment);
            createPO.PressFinalizarCompra();

            // --- PASO 3: DETALLES (CONFIRMACIÓN) ---
            // Verificamos que llegamos a la página de detalles y los datos son correctos
            Assert.True(detailsPO.CheckPurchaseDetail(userName, userSurname, address), "La página de detalles no muestra los datos del cliente correctamente.");
            Assert.True(detailsPO.CheckListOfDevices(expectedItems), "La tabla de detalles no muestra el dispositivo comprado.");
        }

        [Fact(DisplayName = "Esc-2: Filtrar Dispositivos (FA1)")]
        public void Purchase_Filter_Devices()
        {
            var selectPO = new SelectDevice_PO(_driver, null);
            selectPO.Visit();

            // Filtramos por algo que sabemos que existe
            string filterName = "Samsung";
            selectPO.FilterDevices(filterName, "");

            // Verificamos que lo que queda visible contiene "Samsung"
            Assert.True(selectPO.IsDeviceVisible(filterName), "El filtro debería mostrar los dispositivos coincidentes.");
        }

        [Fact(DisplayName = "Esc-2b: Filtro Sin Resultados (FA0)")]
        public void Purchase_Filter_NoResults()
        {
            var selectPO = new SelectDevice_PO(_driver, null);
            selectPO.Visit();

            // Filtramos por algo absurdo
            selectPO.FilterDevices("MovilQueNoExisteXYZ", "");

            // Verificamos que salga la alerta amarilla
            Assert.True(selectPO.CheckMessageErrorNotAvailable("No se encontraron dispositivos"), "Debería aparecer el mensaje de alerta de stock vacío.");
        }

        [Fact(DisplayName = "Esc-3: Carrito Vacío (FA4)")]
        public void Purchase_Cart_Empty()
        {
            var selectPO = new SelectDevice_PO(_driver, null);
            selectPO.Visit();

            // Sin seleccionar nada...
            // Verificamos que el botón de tramitar NO esté disponible
            Assert.True(selectPO.CheckRentButtonDisabledOrMissing(), "El botón 'Tramitar' no debería estar visible/activo si el carrito está vacío.");
        }

        [Fact(DisplayName = "Esc-4: Validación de Formulario (FA5)")]
        public void Purchase_Validation_Error()
        {
            var selectPO = new SelectDevice_PO(_driver, null);
            var createPO = new CreatePurchase_PO(_driver, null);

            selectPO.Visit();
            selectPO.SelectDevice("iPhone 13"); // Necesitamos pasar al paso 2
            selectPO.ClickTramitar();

            // Dejamos campos vacíos y pulsamos finalizar
            createPO.FillInPurchaseInfo("", "", "", "CreditCard"); // Nombre vacío
            createPO.PressFinalizarCompra();

            // Verificamos que aparece el mensaje de error de Blazor
            Assert.True(createPO.CheckValidationError("The CustomerUserName field is required"), "Debería saltar la validación del campo Nombre.");
        }

        [Fact(DisplayName = "Esc-5: Modificar Carrito - Eliminar Ítem (FA3)")]
        public void Purchase_Modify_Cart_Remove()
        {
            var selectPO = new SelectDevice_PO(_driver, null);
            var createPO = new CreatePurchase_PO(_driver, null);

            string device = "iPhone 13";

            selectPO.Visit();
            selectPO.SelectDevice(device);
            selectPO.ClickTramitar();

            // Estamos en Create. Ahora borramos el ítem.
            createPO.RemoveItemFromCart(device);

            // Al vaciar el carrito, tu lógica (según CreateDEVICEScompra.razor) redirige a Selección.
            // Verificamos que hemos vuelto a la selección o que el botón tramitar ha desaparecido.

            // Esperamos un segundo para la redirección
            System.Threading.Thread.Sleep(1000);

            // Si redirige a selección, la URL debe contener "/compra/seleccion"
            Assert.Contains("/compra/seleccion", _driver.Url);
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}