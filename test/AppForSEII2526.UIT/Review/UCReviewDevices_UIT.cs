using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Review {
    public class UCReview_UIT : UC_UIT {
        // --- Datos de Prueba (Constantes) ---
        private const string deviceName1 = "Samsung Galaxy S21";
        private const string deviceBrand1 = "Samsung";
        private const string deviceYear1 = "2021";
        private const string deviceColor1 = "Black";
        private const string deviceModel1 = "SM-G991B";

        private const string deviceName2 = "iPhone 13";
        private const string deviceBrand2 = "Apple";
        private const string deviceYear2 = "2021";

        // Datos para la reseña
        private const string validReviewTitle = "My Tech Experience";
        private const string validUserName = "test@gmail.com";
        private const string validCountry = "Spain";
        private const string commentDevice1 = "Excellent battery life";
        private const string ratingDevice1 = "5"; // String para el SelectByValue
        private const string ratingDevice1Display = "5 / 5"; // Cómo se ve en Details

        private ListDevicesForReview_PO listDevices;

        public UCReview_UIT(ITestOutputHelper output) : base(output) {
            Initial_step_opening_the_web_page();
            listDevices = new ListDevicesForReview_PO(_driver, _output);
        }

        // Precondición: Login (si es necesario para acceder al menú)
        private void Precondition_perform_login() {
            Perform_login("test@gmail.com", "Password123!");
        }

        // Pasos iniciales para llegar a la pantalla de selección
        private void InitialStepsForReview_UIT() {
            Precondition_perform_login();

            // Asumo que hay un enlace en el menú con ID "ReviewLink" o navegas por URL
            // Si tienes un botón en el menú, descomenta la siguiente línea:
            // listDevices.WaitForBeingVisibleIgnoringExeptionTypes(By.Id("ReviewLink"));
            // _driver.FindElement(By.Id("ReviewLink")).Click();

            // Alternativa directa si no tienes el ID del menú a mano:
            _driver.Navigate().GoToUrl(_URI + "Review/SelectDeviceForReview");
        }

        // --- Casos de Prueba ---

        [Theory]
        [InlineData("Samsung", "2021", deviceName1)]
        [InlineData("Apple", "0", deviceName2)] // 0 = Todos los años
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_FilterDevices_AF1(string brand, string year, string expectedDeviceName) {
            // Arrange
            // Esperamos ver al menos el dispositivo buscado en la tabla
            // La estructura del string[] depende de las columnas de tu tabla (Name, Brand, Color, Year, Model, Action)
            // Aquí simplifico verificando que el nombre y la marca coincidan en una fila
            var expectedDevices = new List<string[]>
            {
                new string[] { expectedDeviceName, brand }
            };

            // Act
            InitialStepsForReview_UIT();
            listDevices.FilterDevices(brand, year);

            // Assert
            Assert.True(listDevices.CheckListOfDevices(expectedDevices),
                $"Failed to filter devices by Brand: {brand} and Year: {year}");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ProceedButtonDisabled_WhenNoSelection() {
            // Arrange & Act
            InitialStepsForReview_UIT();
            listDevices.FilterDevices("All", "0"); // Reset filter

            // Assert
            // Al entrar, no hay nada seleccionado, el botón debe estar deshabilitado
            Assert.True(listDevices.CheckProceedButtonDisabled(),
                "Proceed button should be disabled when cart is empty");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ModifySelection_AF4_BackFromCreate() {
            // Arrange
            var createReviewPO = new CreateReview_PO(_driver, _output);

            // Act
            InitialStepsForReview_UIT();

            // 1. Seleccionamos 2 dispositivos
            listDevices.FilterDevices("All", "0");
            listDevices.SelectDevicesByName(new List<string> { deviceName1, deviceName2 });
            listDevices.PressProceedToReview();

            // 2. Estamos en Create, pulsamos "Modify selection"
            createReviewPO.PressModifySelection();

            // 3. De vuelta en la lista, quitamos uno
            listDevices.RemoveDeviceFromCart(deviceName2);

            // 4. Volvemos a ir a Create
            listDevices.PressProceedToReview();

            // Assert
            // Verificar que en la tabla de Create solo queda 1 dispositivo
            var expectedReviewItems = new List<string[]>
            {
                new string[] { deviceName1, deviceYear1, deviceModel1 }
            };

            Assert.True(createReviewPO.CheckListOfReviewItems(expectedReviewItems),
                "The list of devices to review did not update correctly after modification.");
        }

        [Theory]
        [InlineData("", "Spain", "Review Title is required")] // Título vacío (asumiendo validación HTML5 o DataAnnotations)
        [InlineData("My Title", "0", "Country field is required")] // País no seleccionado (Value="0")
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ValidationErrors_AF3(string title, string country, string expectedErrorPartial) {
            // Arrange
            var createReviewPO = new CreateReview_PO(_driver, _output);

            // Act
            InitialStepsForReview_UIT();

            // Seleccionar y avanzar
            listDevices.FilterDevices("All", "0");
            listDevices.SelectDevicesByName(new List<string> { deviceName1 });
            listDevices.PressProceedToReview();

            // Rellenar formulario con datos inválidos
            // Nota: Si el País es "0", en el PO pasamos el texto del option por defecto "Select country..."
            string countryText = country == "0" ? "Select country..." : country;

            createReviewPO.FillInReviewInfo(title, countryText, validUserName);

            // Rellenar datos del item (obligatorios para que no falle por esto)
            // Necesitamos el ID del dispositivo. En UIT es difícil saber el ID numérico.
            // *Solución*: En el PO CreateReview_PO deberías tener un método para rellenar el "primer" item o todos.
            // *Hack para este ejemplo*: Asumimos que fillFeedback busca por ID, pero si es un test funcional puro,
            // deberíamos haber extraído el ID de la URL o del DOM. 
            // Para simplificar, asumiremos que FillInDeviceFeedback itera o le pasamos un ID dummy si el PO lo soporta, 
            // o asumimos que fallará la validación global antes.
            // Si la validación salta al pulsar Save, pulsamos Save:

            createReviewPO.PressSaveReview();

            // Assert
            // CheckValidationError busca en el ValidationSummary o en los mensajes de error
            // Como tu CreateReview_PO usa PageSource o ErrorsShown:
            // Ajusta el mensaje esperado según tus DataAnnotations reales (ej: "The ReviewTitle field is required")
            Assert.True(createReviewPO.CheckValidationError(expectedErrorPartial) || _driver.PageSource.Contains(expectedErrorPartial),
                $"Expected error message '{expectedErrorPartial}' not found.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_BasicFlow_HappyPath() {
            // Arrange
            var createReviewPO = new CreateReview_PO(_driver, _output);
            var detailReviewPO = new DetailReview_PO(_driver, _output);

            // Act
            // 1. Listado y Selección
            InitialStepsForReview_UIT();
            listDevices.FilterDevices("Samsung", "2021");
            listDevices.SelectDevicesByName(new List<string> { deviceName1 });

            // Verificamos estado visual antes de avanzar
            Assert.True(listDevices.CheckDeviceMarkedAsAdded(deviceName1));

            listDevices.PressProceedToReview();

            // 2. Creación (Formulario)
            createReviewPO.FillInReviewInfo(validReviewTitle, validCountry, validUserName);

            // IMPORTANTE: Aquí necesitamos el ID del dispositivo para rellenar la fila en CreateReview_PO.
            // Como en el test no sabemos el ID de BDD (ej: 10), una estrategia común en UIT es:
            // A) Tener métodos en el PO que digan "Rellena la primera fila".
            // B) O buscar el ID en el DOM antes.
            // Vamos a asumir un método 'FillFeedbackForFirstDevice' o que sabes el ID por datos semilla.
            // Si usas datos semilla (SeedData), sabes que Samsung S21 es ID 1, por ejemplo.
            // Supongamos ID = 1 para este ejemplo.
            int deviceIdSeed = 1;
            createReviewPO.FillInDeviceFeedback(deviceIdSeed, commentDevice1, ratingDevice1);

            createReviewPO.PressSaveReview();
            createReviewPO.ConfirmDialog(); // Confirmar Modal

            // 3. Detalles (Verificación)

            // Verificar Cabecera
            Assert.True(detailReviewPO.CheckReviewDetails(validReviewTitle, DateTime.Now, validUserName, validCountry),
                "Review Header details are incorrect.");

            // Verificar Tabla de items
            var expectedDetailsItems = new List<string[]>
            { 
                // Name, Model, Year, Score (5 / 5), Comment ("comentario")
                new string[] { deviceName1, deviceModel1, deviceYear1, ratingDevice1Display, $"\"{commentDevice1}\"" }
            };

            Assert.True(detailReviewPO.CheckListOfDevices(expectedDetailsItems),
                "Review Items in Detail view match the expected input.");
        }
    }
}