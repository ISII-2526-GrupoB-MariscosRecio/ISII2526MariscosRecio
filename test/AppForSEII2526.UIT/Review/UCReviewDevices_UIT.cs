using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using AppForSEII2526.UIT.Shared;

namespace AppForSEII2526.UIT.Review {
    public class UCReview_UIT : UC_UIT {
        // --- Datos de Prueba ---
        private const int defaultDeviceId = 1; // Galaxy S10
        private const string deviceName1 = "Galaxy S10";
        private const string deviceBrand1 = "Samsung";
        private const string deviceYear1 = "2019";
        private const string deviceModel1 = "Samsung";

        private const string deviceName2 = "iPhone 11";

        private const string validReviewTitle = "Experiencia de usuario";
        private const string validUserName = "test@gmail.com";
        private const string validCountry = "Spain";

        // Requisito: Comentario debe empezar por "Reseña para"
        private const string commentDevice1 = "Reseña para: Buen rendimiento y batería duradera.";
        private const string ratingDevice1 = "5";
        private const string ratingDevice1Display = "5 / 5";

        private ListDevicesForReview_PO listDevices;

        public UCReview_UIT(ITestOutputHelper output) : base(output) {
            Initial_step_opening_the_web_page();
            listDevices = new ListDevicesForReview_PO(_driver, _output);
        }

        private void Precondition_perform_login() {
            Perform_login("test@gmail.com", "Password123!");
        }

        private void InitialStepsForReview_UIT() {
            Precondition_perform_login();
            _driver.Navigate().GoToUrl(_URI + "Review/SelectDeviceForReview");
        }

        // --- Casos de Prueba ---

        [Theory]
        [InlineData("Samsung", "2019", deviceName1)]
        [InlineData("Apple", "0", deviceName2)]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_FilterDevices_AF1(string brand, string year, string expectedDeviceName) {
            var expectedDevices = new List<string[]> { new string[] { expectedDeviceName, brand } };
            InitialStepsForReview_UIT();
            listDevices.FilterDevices(brand, year);
            Assert.True(listDevices.CheckListOfDevices(expectedDevices), $"Fallo filtro Marca: {brand}, Año: {year}");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ProceedButtonDisabled_WhenNoSelection() {
            InitialStepsForReview_UIT();
            listDevices.FilterDevices("All", "0");
            Assert.True(listDevices.CheckProceedButtonDisabled(), "Botón Proceed debería estar deshabilitado.");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ModifySelection_AF4_BackFromCreate() {
            var createReviewPO = new CreateReview_PO(_driver, _output);
            InitialStepsForReview_UIT();

            listDevices.FilterDevices("All", "0");
            listDevices.SelectDevicesByName(new List<string> { deviceName1, deviceName2 });
            listDevices.PressProceedToReview();

            createReviewPO.PressModifySelection();

            listDevices.RemoveDeviceFromCart(deviceName2);
            listDevices.PressProceedToReview();

            var expectedReviewItems = new List<string[]> { new string[] { deviceName1, deviceYear1, deviceModel1 } };
            Assert.True(createReviewPO.CheckListOfReviewItems(expectedReviewItems), "La lista no se actualizó tras modificar.");
        }

        [Theory]
        // CORREGIDO: Mensajes exactos definidos en ReviewForCreateDTO.cs
        [InlineData("", "Spain", "The ReviewTitle field is required.")]
        // Buscamos solo la frase clave, evitando símbolos y listas largas que rompen el string comparison
        [InlineData("My Title", "0", "The country is not valid")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ValidationErrors_AF3(string title, string country, string expectedErrorPartial) {
            var createReviewPO = new CreateReview_PO(_driver, _output);
            InitialStepsForReview_UIT();

            listDevices.FilterDevices("All", "0");
            listDevices.SelectDevicesByName(new List<string> { deviceName1 });
            listDevices.PressProceedToReview();

            string countryText = country == "0" ? "Select country..." : country;

            createReviewPO.FillInReviewInfo(title, countryText, validUserName);
            // Llenamos el feedback del dispositivo para que el único error sea el del formulario principal
            createReviewPO.FillInDeviceFeedback(defaultDeviceId, commentDevice1, ratingDevice1);

            createReviewPO.PressSaveReview();

            // CheckValidationError ahora es capaz de leer el ValidationSummary
            Assert.True(createReviewPO.CheckValidationError(expectedErrorPartial) || _driver.PageSource.Contains(expectedErrorPartial),
                $"No se encontró el error esperado: '{expectedErrorPartial}'");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_BasicFlow_HappyPath() {
            var createReviewPO = new CreateReview_PO(_driver, _output);
            var detailReviewPO = new DetailReview_PO(_driver, _output);

            InitialStepsForReview_UIT();
            listDevices.FilterDevices(deviceBrand1, deviceYear1);
            listDevices.SelectDevicesByName(new List<string> { deviceName1 });

            listDevices.PressProceedToReview();

            createReviewPO.FillInReviewInfo(validReviewTitle, validCountry, validUserName);
            createReviewPO.FillInDeviceFeedback(defaultDeviceId, commentDevice1, ratingDevice1);

            createReviewPO.PressSaveReview();

            createReviewPO.ConfirmDialog();

            // Verificamos que no haya errores de API antes de continuar (evita Timeouts silenciosos)
            createReviewPO.AssertNoErrors();

            Assert.True(detailReviewPO.CheckReviewDetails(validReviewTitle, DateTime.Now, validUserName, validCountry),
                "No se cargaron los detalles de la reseña correctamente.");

            var expectedDetailsItems = new List<string[]>
            {
                new string[] { deviceName1, deviceModel1, deviceYear1, ratingDevice1Display, $"\"{commentDevice1}\"" }
            };
            Assert.True(detailReviewPO.CheckListOfDevices(expectedDetailsItems),
                "Los items en la tabla de detalles no coinciden.");
        }
    }
}