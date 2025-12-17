using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Review {
    public class CreateReview_PO : PageObject {
        // --- Elementos Fijos ---
        private By _titleBy = By.Id("Title");
        private IWebElement _title() => _driver.FindElement(_titleBy);

        private IWebElement _country() => _driver.FindElement(By.Id("Country"));

        private IWebElement _customerName() => _driver.FindElement(By.Id("CustomerName"));

        private IWebElement _submitButton() => _driver.FindElement(By.Id("Submit"));

        private IWebElement _modifyButton() => _driver.FindElement(By.Id("ModifyDevices"));

        // Elemento de error general
        private By _errorLabelBy = By.Id("ErrorsShown");

        public CreateReview_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) {
        }

        // --- Acciones Principales ---

        public void FillInReviewInfo(string title, string country, string customerName) {
            WaitForBeingVisible(_titleBy);
            _title().SendKeys(title);
            _customerName().SendKeys(customerName);

            // Manejo del Dropdown de País
            SelectElement selectCountry = new SelectElement(_country());
            // Seleccionamos por texto visible (ej: "Spain", "France") tal como está en el <option>
            selectCountry.SelectByText(country);
        }

        // Método para rellenar los datos de cada dispositivo en la tabla
        // Como los inputs están dentro de un bucle, los buscamos relativos a la fila del ID del dispositivo
        public void FillInDeviceFeedback(int deviceId, string comment, string ratingValue) {
            // 1. Encontramos la fila específica por su ID dinámico
            var row = _driver.FindElement(By.Id($"DeviceRow_{deviceId}"));

            // 2. Buscamos el textArea DENTRO de esa fila (es el único textarea en la fila)
            var commentInput = row.FindElement(By.TagName("textarea"));
            commentInput.Clear();
            commentInput.SendKeys(comment);

            // 3. Buscamos el select DENTRO de esa fila (es el único select en la fila)
            var ratingSelectElem = row.FindElement(By.TagName("select"));
            SelectElement selectRating = new SelectElement(ratingSelectElem);

            // Seleccionamos por Valor (ej: "1", "2", ... "5") ya que es más seguro que el texto largo
            selectRating.SelectByValue(ratingValue);
        }

        public void PressSaveReview() {
            _submitButton().Click();
        }

        public void PressModifySelection() {
            _modifyButton().Click();
        }

        public void RemoveDeviceFromList(int deviceId) {
            // Busca el botón "Remove" dentro de la fila específica
            var row = _driver.FindElement(By.Id($"DeviceRow_{deviceId}"));
            // Buscamos el botón por la clase css o texto, ya que no tiene ID único, 
            // pero al buscar dentro de 'row' es seguro.
            row.FindElement(By.CssSelector(".btn-danger")).Click();
        }

        // --- Manejo del Dialog/Modal ---
        // Asumiendo que el componente <Dialog> renderiza un modal estándar.
        // Quizás necesites ajustar el selector dependiendo de cómo se renderice el botón "Yes/OK" en tu HTML final.
        public void ConfirmDialog() {
            // Espera pequeña para que el modal aparezca (o usa WaitForBeingVisible si el modal tiene ID)
            // Aquí busco un botón genérico de confirmación dentro del modal
            // Ajusta el XPath o CSS según tu componente 'Dialog.razor'
            var confirmButton = _driver.FindElement(By.XPath("//div[@class='modal-footer']//button[contains(@class, 'btn-primary')]"));
            confirmButton.Click();
        }

        // --- Validaciones ---

        public bool CheckListOfReviewItems(List<string[]> expectedItems) {
            return CheckBodyTable(expectedItems, By.Id("TableOfReviewItems"));
        }

        public bool CheckValidationError(string expectedError) {
            // Opción A: Buscar en el PageSource (rápido y genérico)
            // return _driver.PageSource.Contains(expectedError);

            // Opción B: Buscar en el elemento específico de errores (más preciso)
            WaitForBeingVisible(_errorLabelBy);
            return _driver.FindElement(_errorLabelBy).Text.Contains(expectedError);
        }
    }
}