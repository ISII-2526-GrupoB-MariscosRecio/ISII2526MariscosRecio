using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using AppForSEII2526.UIT.Shared;

namespace AppForSEII2526.UIT.Review {
    public class CreateReview_PO : PageObject {

        // Elementos Fijos
        private By _titleBy = By.Id("Title");
        private IWebElement _title() => _driver.FindElement(_titleBy);
        private IWebElement _country() => _driver.FindElement(By.Id("Country"));
        private IWebElement _customerName() => _driver.FindElement(By.Id("CustomerName"));
        private IWebElement _submitButton() => _driver.FindElement(By.Id("Submit"));
        private IWebElement _modifyButton() => _driver.FindElement(By.Id("ModifyDevices"));

        // Elementos del Dialog y Errores
        private By _dialogOkButtonBy = By.Id("Button_DialogOK");
        private By _dialogModalBy = By.Id("DialogOKSaveDelete");
        private By _errorLabelBy = By.Id("ErrorsShown");

        public CreateReview_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

        public void FillInReviewInfo(string title, string country, string customerName) {
            WaitForBeingVisible(_titleBy);
            _title().SendKeys(title);
            _customerName().SendKeys(customerName);

            // FIX: Evita seleccionar la opción "Select country..." que está deshabilitada en el HTML
            if (!string.IsNullOrEmpty(country) && country != "Select country...") {
                SelectElement selectCountry = new SelectElement(_country());
                selectCountry.SelectByText(country);
            }
        }

        public void FillInDeviceFeedback(int deviceId, string comment, string ratingValue) {
            var row = _driver.FindElement(By.Id($"DeviceRow_{deviceId}"));
            var commentInput = row.FindElement(By.TagName("textarea"));
            commentInput.Clear();
            commentInput.SendKeys(comment);

            var ratingSelectElem = row.FindElement(By.TagName("select"));
            new SelectElement(ratingSelectElem).SelectByValue(ratingValue);
        }

        public void PressSaveReview() {
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", _submitButton());
            _submitButton().Click();
        }

        public void PressModifySelection() {
            _modifyButton().Click();
        }

        public void ConfirmDialog() {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));

            // 1. Clic en OK
            var okButton = wait.Until(ExpectedConditions.ElementToBeClickable(_dialogOkButtonBy));
            okButton.Click();

            // 2. Esperar a que el modal DESAPAREZCA (Crucial para Blazor)
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(_dialogModalBy));
        }

        // --- Validaciones ---

        public bool CheckListOfReviewItems(List<string[]> expectedItems) {
            return CheckBodyTable(expectedItems, By.Id("TableOfReviewItems"));
        }

        // FIX: Busca el mensaje en cualquier alerta visible (cubre ValidationSummary y ErrorsShown)
        public bool CheckValidationError(string expectedError) {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            try {
                return wait.Until(d => {
                    // Busca elementos con clase 'alert' que contengan el texto
                    var alerts = d.FindElements(By.XPath($"//*[contains(@class, 'alert') and contains(., '{expectedError}')]"));
                    return alerts.Any(a => a.Displayed);
                });
            }
            catch (WebDriverTimeoutException) {
                return false;
            }
        }

        public void AssertNoErrors() {
            try {
                var errorElement = _driver.FindElement(_errorLabelBy);
                // Si el div es visible y tiene más texto que el label "Errors:"
                if (errorElement.Displayed && errorElement.Text.Replace("Errors:", "").Trim().Length > 0) {
                    throw new Exception($"Error de aplicación detectado: {errorElement.Text}");
                }
            }
            catch (NoSuchElementException) { }
        }
    }
}