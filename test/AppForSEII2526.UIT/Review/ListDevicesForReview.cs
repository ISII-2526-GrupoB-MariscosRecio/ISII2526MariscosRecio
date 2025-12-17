using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Review {
    public class ListDevicesForReview_PO : PageObject {
        // --- Selectores (IDs exactos de tu Razor) ---
        private By _brandSelectBy = By.Id("selectBrand");
        private By _yearInputBy = By.Id("inputYear");
        private By _searchButtonBy = By.Id("searchDevices");
        private By _tableOfDevicesBy = By.Id("TableOfDevices");
        private By _proceedButtonBy = By.Id("startReviewButton");
        private By _errorLabelBy = By.Id("ErrorsShown");

        // --- WebElements ---
        private IWebElement _brandSelect() => _driver.FindElement(_brandSelectBy);
        private IWebElement _yearInput() => _driver.FindElement(_yearInputBy);
        private IWebElement _searchButton() => _driver.FindElement(_searchButtonBy);
        private IWebElement _proceedButton() => _driver.FindElement(_proceedButtonBy);

        public ListDevicesForReview_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output) {
        }

        // --- Acciones ---

        public void FilterDevices(string brand, string year) {
            WaitForBeingVisible(_brandSelectBy);

            // 1. Filtrar por Marca (Select)
            if (string.IsNullOrEmpty(brand)) brand = "All Brands"; // O "All" según el value del option
            SelectElement selectElement = new SelectElement(_brandSelect());

            // Usamos SelectByValue si el input es "All", o SelectByText si es el nombre visible.
            // En tu Razor value="@brandName", así que el texto y el valor suelen coincidir.
            try {
                selectElement.SelectByValue(brand);
            }
            catch {
                selectElement.SelectByText(brand);
            }

            // 2. Filtrar por Año (Input Number)
            _yearInput().Clear();
            if (!string.IsNullOrEmpty(year) && year != "0") {
                _yearInput().SendKeys(year);
            }

            // 3. Buscar
            _searchButton().Click();

            // Espera técnica para que la tabla se recargue (simulado según tu estilo anterior)
            Thread.Sleep(1000);
        }

        public void SelectDevicesByName(List<string> deviceNames) {
            // Como en el Razor el ID es numérico (deviceToReview_1, deviceToReview_2...),
            // pero el test pasa nombres ("iPhone 13"), usamos XPath para encontrar la fila
            // que contiene el nombre y luego buscamos el botón dentro de esa fila.

            foreach (var name in deviceNames) {
                // XPath: Busca un <tr> que tenga un <td> con el texto exacto del nombre,
                // luego busca dentro el botón que contenga 'deviceToReview_' en su id.
                var xpathButton = $"//tr[td[normalize-space()='{name}']]//button[contains(@id, 'deviceToReview_')]";

                // Esperamos que aparezca la fila con ese nombre
                WaitForBeingVisible(By.XPath(xpathButton));

                var button = _driver.FindElement(By.XPath(xpathButton));

                // Solo clicamos si no está deshabilitado (ya añadido)
                if (button.Enabled) {
                    button.Click();
                    // Pequeña espera para que se actualice el carrito visualmente
                    Thread.Sleep(200);
                }
            }
        }

        public void RemoveDeviceFromCart(string deviceName) {
            // Similar al anterior, buscamos en la lista lateral (Cart) por el nombre
            // Tu Razor: <li ...><small class="fw-bold">@item.DeviceName</small> ... <button id="removeDevice_...">

            var xpathRemoveBtn = $"//li[.//small[normalize-space()='{deviceName}']]//button[contains(@id, 'removeDevice_')]";

            WaitForBeingVisible(By.XPath(xpathRemoveBtn));
            _driver.FindElement(By.XPath(xpathRemoveBtn)).Click();
        }

        public void ClearCart() {
            // El botón Clear no tiene ID en tu snippet, pero tiene la clase 'btn-danger' y title='Remove All'
            // O podemos buscar por texto "Clear".
            var clearBtnBy = By.XPath("//button[contains(text(), 'Clear')]");
            if (_driver.FindElements(clearBtnBy).Count > 0) {
                _driver.FindElement(clearBtnBy).Click();
            }
        }

        public void PressProceedToReview() {
            WaitForBeingClickable(_proceedButtonBy);
            _proceedButton().Click();
        }

        // --- Validaciones ---

        public bool CheckListOfDevices(List<string[]> expectedDevices) {
            // Verifica el contenido de la tabla principal
            return CheckBodyTable(expectedDevices, _tableOfDevicesBy);
        }

        public bool CheckProceedButtonDisabled() {
            // Devuelve true si el botón NO está habilitado
            return !(_proceedButton().Enabled);
        }

        public bool CheckDeviceMarkedAsAdded(string deviceName) {
            // Verifica que el botón de ese dispositivo haya cambiado a "Added" (verde y deshabilitado)
            // Buscamos la fila por nombre
            var xpathButton = $"//tr[td[normalize-space()='{deviceName}']]//button";
            var button = _driver.FindElement(By.XPath(xpathButton));

            // En tu Razor, si está añadido tiene clase 'btn-success' y texto 'Added'
            return button.Text.Contains("Added") && !button.Enabled;
        }

        public bool CheckMessageError(string expectedError) {
            // Verifica el div de error superior
            // Como el div tiene hidden="@hideErrors", Selenium puede no encontrarlo visible si no hay error.
            try {
                if (_driver.FindElement(_errorLabelBy).Displayed) {
                    return _driver.FindElement(_errorLabelBy).Text.Contains(expectedError);
                }
                return false;
            }
            catch (NoSuchElementException) {
                return false;
            }
        }
    }
}