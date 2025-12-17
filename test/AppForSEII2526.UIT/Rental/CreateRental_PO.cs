using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using AppForSEII2526.UIT.Shared;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Rental {
    public class CreateRental_PO : PageObject {
        // IDs estándar del formulario de creación
        private By _nameInput = By.Id("CustomerUserName");
        private By _surnameInput = By.Id("NameSurname");
        private By _addressInput = By.Id("DeliveryAddress");
        private By _paymentSelect = By.Id("PaymentMethod");
        // Botones por texto (XPath) para evitar problemas si no tienen ID
        private By _submitButton = By.XPath("//button[contains(text(), 'Rent your devices')]");
        private By _modifyButton = By.XPath("//button[contains(text(), 'Modify devices')]");

        public CreateRental_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

        public void FillInRentalInfo(string name, string surname, string address, string payment) {
            WaitForBeingVisible(_nameInput);

            _driver.FindElement(_nameInput).Clear();
            _driver.FindElement(_nameInput).SendKeys(name);

            _driver.FindElement(_surnameInput).Clear();
            _driver.FindElement(_surnameInput).SendKeys(surname);

            _driver.FindElement(_addressInput).Clear();
            _driver.FindElement(_addressInput).SendKeys(address);

            new SelectElement(_driver.FindElement(_paymentSelect)).SelectByText(payment);
        }

        public void PressRentYourDevices() {
            WaitForBeingClickable(_submitButton);
            _driver.FindElement(_submitButton).Click();
        }

        public void PressModifyDevices() {
            WaitForBeingVisible(_modifyButton);
            _driver.FindElement(_modifyButton).Click();
        }

        public void PressOkModalDialog() {
            try {
                // Busca el botón primario en el modal
                var btn = _driver.FindElement(By.CssSelector(".modal-footer .btn-primary"));
                if (btn.Displayed) btn.Click();
            }
            catch (NoSuchElementException) { /* Modal no apareció o no es necesario */ }
        }

        // --- MÉTODO NUEVO (El que faltaba) ---
        public bool CheckListOfRentalItems(List<string[]> expectedRentalItems) {
            // Verifica la tabla resumen en la pantalla de confirmación
            // El ID 'TableOfRentalItems' es el estándar en estos casos
            return CheckBodyTable(expectedRentalItems, By.Id("TableOfRentalItems"));
        }
        // -------------------------------------

        public bool CheckValidationError(string error) => _driver.PageSource.Contains(error);
    }
}