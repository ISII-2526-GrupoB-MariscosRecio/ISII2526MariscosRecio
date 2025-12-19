using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using AppForSEII2526.UIT.Shared;
using System.Security.Cryptography.X509Certificates;

namespace AppForSEII2526.UIT.Rental {
    public class UCRental_UIT : UC_UIT {
        // --- Datos de Prueba (Basados en tu captura de pantalla) ---
        private const string deviceName1 = "Galaxy S10";
        private const string deviceBrand1 = "Samsung";
        private const string deviceName3 = "iPhone 11";
        private const string deviceBrand3 = "Apple";

        // Datos para filtrado que sabemos que existen
        private const string deviceName2 = "iPhone 11";

        // Datos de usuario para el formulario de Rental
        private const string validName = "iker";
        private const string validSurname = "Garcia";
        private const string validAddress = "Calle Universidad 1";
        private const string validPayment = "Credit Card"; // Asegúrate que este texto coincide con el <option> de tu HTML

        // Fechas para el alquiler (Dinámicas para que no caduquen)
        private DateTime dateFrom = DateTime.Today.AddDays(1);
        private DateTime dateTo = DateTime.Today.AddDays(5);

        private ListDevicesForRental_PO listDevices;

        public UCRental_UIT(ITestOutputHelper output) : base(output) {
            Initial_step_opening_the_web_page();
            listDevices = new ListDevicesForRental_PO(_driver, _output);
        }

        private void Precondition_perform_login() {
            // CREDENCIALES CORREGIDAS SEGÚN TU ÚLTIMO MENSAJE
            Perform_login("testin@mail.com", "Iker12@");

            // PAUSA VITAL: Esperamos a que la sesión se asiente para evitar AggregateException
            System.Threading.Thread.Sleep(2000);
        }

        private void InitialStepsForRental_UIT() {
            Precondition_perform_login();
            // Navegación explícita a la URL que se ve en tu navegador
            _driver.Navigate().GoToUrl(_URI + "Rental/selectdevices");
        }

        // --- Casos de Prueba ---

        [Theory]
        [InlineData("Samsung", "100", deviceName1)] // Filtro: Marca Samsung, Precio Max 100
        [InlineData("Apple", "1000", deviceName2)]  // Filtro: Marca Apple, Precio Max 1000
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_FilterDevices_AF1(string model, string maxPrice, string expectedDeviceName) {
            // Esperamos ver al menos el nombre del dispositivo
            var expectedDevices = new List<string[]> { new string[] { expectedDeviceName } };

            InitialStepsForRental_UIT();

            // Aplicamos filtros
            //listDevices.FilterDevices(model, maxPrice, dateFrom, dateTo);

            // Verificamos
            Assert.True(listDevices.CheckListOfDevices(expectedDevices),
                $"Fallo AF1: No se encontraron dispositivos filtrando por '{model}' y precio '{maxPrice}'.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ProceedButtonDisabled_WhenNoSelection() {
            InitialStepsForRental_UIT();
            // Filtramos pero no seleccionamos nada
            //listDevices.FilterDevices("All", "", dateFrom, dateTo);

            // El botón debe estar deshabilitado o no visible
            Assert.True(listDevices.CheckRentDevicesDisabled(),
                "El botón 'Confirm Rental' debería estar deshabilitado/oculto si el carrito está vacío.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ModifySelection_AF4_BackFromCreate() {
            var createRentalPO = new CreateRental_PO(_driver, _output);
            InitialStepsForRental_UIT();

            // 1. Seleccionar 2 dispositivos
            //listDevices.FilterDevices("", "", dateFrom, dateTo);
            listDevices.SelectDevices(new List<string> { deviceName1, deviceName2 });
            listDevices.RentDevices(); // Ir a pantalla Create

            // 2. Modificar (Volver atrás a la lista)
            createRentalPO.PressModifyDevices();

            // 3. Quitar uno (iPhone 11)
            listDevices.ModifyRentingCart(deviceName2);

            // 4. Volver a avanzar
            listDevices.RentDevices();

            // Assert: En la tabla resumen (Create) solo debe quedar el Galaxy S10
            var expectedRentalItems = new List<string[]> { new string[] { deviceName1 } };
            Assert.True(createRentalPO.CheckListOfRentalItems(expectedRentalItems),
                "La lista no se actualizó correctamente tras eliminar un dispositivo y volver.");
        }

        [Theory]
        // Mensajes de error estándar de MVC (ajusta si los tuyos son personalizados en español)
        [InlineData("", "Test", "Calle Falsa", "Credit Card", "The Name field is required")]
        [InlineData("Fernando", "", "Calle Falsa", "Credit Card", "The Surname field is required")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ValidationErrors_AF3(string name, string surname, string address, string payment, string expectedErrorPartial) {
            var createRentalPO = new CreateRental_PO(_driver, _output);
            InitialStepsForRental_UIT();

            // Llegar al formulario
            //listDevices.FilterDevices("", "", dateFrom, dateTo);
            listDevices.SelectDevices(new List<string> { deviceName1 });
            listDevices.RentDevices();

            // Rellenar con datos inválidos
            //createRentalPO.FillInRentalInfo(name, surname, address, payment);

            createRentalPO.PressRentYourDevices();

            // Verificar error
            Assert.True(createRentalPO.CheckValidationError(expectedErrorPartial) || _driver.PageSource.Contains(expectedErrorPartial),
                $"No se encontró el error de validación esperado: '{expectedErrorPartial}'");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_BasicFlow_HappyPath() {
            var createRentalPO = new CreateRental_PO(_driver, _output);
            var detailRentalPO = new DetailRental_PO(_driver, _output);

            // 1. Listado y Selección
            InitialStepsForRental_UIT();

            
            //listDevices.FilterDevices(deviceBrand1, "", dateFrom, dateTo); // Filtrar por Samsung
            listDevices.SelectDevices(new List<string> { deviceName1 });   // Seleccionar Galaxy S10


            



            listDevices.RentDevices(); // Clic en Confirm Rental

            // 2. Creación (Formulario)
            //createRentalPO.FillInRentalInfo(validName, validSurname, validAddress, validPayment);

            createRentalPO.PressRentYourDevices(); // Guardar

            // Confirmación (Si hay modal)
            createRentalPO.PressOkModalDialog();

            // 3. Detalles (Verificar resultado final)
            // Verificamos datos del usuario
            Assert.True(detailRentalPO.CheckRentalDetail(validSurname, validAddress, validPayment, DateTime.Now, dateFrom, dateTo, "€"),
                "No se cargaron los detalles del alquiler correctamente (Nombre/Dirección/Fechas).");

            // Verificamos que el dispositivo esté en la tabla de detalles
            var expectedDetailsItems = new List<string[]>
            {
                new string[] { deviceName1 }
            };
            Assert.True(detailRentalPO.CheckListOfDevices(expectedDetailsItems),
                "Los items en la tabla de detalles final no coinciden con lo alquilado.");
        }
        //EXAMEN
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Examen()
        {
            var createRentalPO = new CreateRental_PO(_driver, _output);
            var detailRentalPO = new DetailRental_PO(_driver, _output);

            // 1. Listado y Selección
            InitialStepsForRental_UIT();
            listDevices.SelectDevices(new List<string> { deviceName3 });
            listDevices.FilterDevices(deviceBrand1, ""); // Filtrar por Samsung
            listDevices.SelectDevices(new List<string> { deviceName1 });   // Seleccionar Galaxy S10

            listDevices.ModifyRentingCart(deviceName3); 

            listDevices.RentDevices(); // Clic en Confirm Rental

            // 2. Creación (Formulario)
            createRentalPO.FillInRentalInfo(validName, validSurname, validAddress);

            createRentalPO.PressRentYourDevices(); // Guardar

            // Confirmación (Si hay modal)
            createRentalPO.PressOkModalDialog();

            // 3. Detalles (Verificar resultado final)
            // Verificamos datos del usuario
            Assert.True(detailRentalPO.CheckRentalDetail(validSurname, validAddress, validPayment, DateTime.Now, dateFrom, dateTo, "€"),
                "No se cargaron los detalles del alquiler correctamente (Nombre/Dirección/Fechas).");

            // Verificamos que el dispositivo esté en la tabla de detalles
            var expectedDetailsItems = new List<string[]>
            {
                new string[] { deviceName1 }
            };
            Assert.True(detailRentalPO.CheckListOfDevices(expectedDetailsItems),
                "Los items en la tabla de detalles final no coinciden con lo alquilado.");
        }
    }
}