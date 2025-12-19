using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using AppForSEII2526.UIT.Shared;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Rental {
    public class ListDevicesForRental_PO : PageObject {
        // --- SELECTORES MÁS ROBUSTOS ---

        // Buscamos inputs genéricos dentro del formulario de filtro si no tienen ID
        // Intenta buscar por ID si existen en tu HTML (ej: id="modelFilter"), si no, usamos Xpath genérico
        private By _inputModelBy = By.Id("filtroModel");

        // El precio suele ser el segundo input o tener label "Price"
        private By _inputPriceBy = By.Id("filtroPrecio");

        // Fechas: Buscamos por type='date'
        private By _fromBy = By.XPath("(//input[@type='date'])[1]");
        private By _toBy = By.XPath("(//input[@type='date'])[2]");

        // Botones: Buscamos por texto en Inglés O Español (para evitar fallos de idioma)
        private By _searchButtonBy = By.Id("btnBuscar");
        private By _rentButtonBy = By.Id("btnConfirmarRent"); 

        private By _tableOfDevicesBy = By.TagName("table");
        private By _errorAreaBy = By.Id("ErrorsShown");

        private By _botonborrar = By.Id("btnBorrar");
        public ListDevicesForRental_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

        public void FilterDevices(string model, string price) {
            // Esperamos a que el botón de búsqueda sea visible para asegurar que la página cargó
            WaitForBeingVisible(_searchButtonBy);
            System.Threading.Thread.Sleep(500); // Pequeña pausa de estabilidad

            // Rellenar Modelo
            if (!string.IsNullOrEmpty(model)) {
                var modelInput = _driver.FindElement(_inputModelBy);
                modelInput.Clear();
                modelInput.SendKeys(model);
            }

            // Rellenar Precio
            if (!string.IsNullOrEmpty(price)) {
                try {
                    var priceInput = _driver.FindElement(_inputPriceBy);
                    priceInput.Clear();
                    priceInput.SendKeys(price);
                }
                catch (NoSuchElementException) {
                    // Si no encuentra el de precio, ignoramos (a veces es un slider)
                }
            }

            // Rellenar Fechas (Truco: SendKeys con formato local suele funcionar mejor)
            // Si falla, prueba con .ToString("dd/MM/yyyy")
            //var fromInput = _driver.FindElement(_fromBy);
            //fromInput.SendKeys(from.ToString("dd/MM/yyyy"));

            //var toInput = _driver.FindElement(_toBy);
            //toInput.SendKeys(to.ToString("dd/MM/yyyy"));

            // Click Buscar
            _driver.FindElement(_searchButtonBy).Click();

            // Esperar a que recargue la tabla (pausa obligada por ser test funcional simple)
            System.Threading.Thread.Sleep(1500);
        }

        public void SelectDevices(List<string> deviceNames) {
            foreach (var name in deviceNames) {
                // XPath corregido: Busca una fila (tr) que tenga el nombre, y dentro busca un botón o enlace "Add"/"Añadir"
                string xpathAddBtn = $"//tr[contains(., '{name}')]//button[contains(text(), 'Add') or contains(text(), 'Añadir') or contains(text(), '+')] | //tr[contains(., '{name}')]//a[contains(text(), 'Add') or contains(text(), 'Añadir')]";

                WaitForBeingVisible(By.XPath(xpathAddBtn));
                _driver.FindElement(By.XPath(xpathAddBtn)).Click();
                System.Threading.Thread.Sleep(500);
            }
        }

        public void RentDevices() {
            WaitForBeingClickable(_rentButtonBy);
            _driver.FindElement(_rentButtonBy).Click();
        }

        public void ModifyRentingCart(string deviceName) {
            // Busca el botón de eliminar (X o Remove) al lado del nombre del dispositivo en el carrito
            //string xpathRemoveBtn = $"//li[contains(., '{deviceName}')]//button | //tr[contains(., '{deviceName}')]//button[contains(text(),'X') or contains(text(),'Remove')]";
            WaitForBeingVisible(_botonborrar);
            _driver.FindElement(_botonborrar).Click();
            System.Threading.Thread.Sleep(500);
        }

        public bool CheckListOfDevices(List<string[]> expectedDevices) => CheckBodyTable(expectedDevices, _tableOfDevicesBy);

        public bool CheckRentDevicesDisabled() {
            try {
                var btn = _driver.FindElement(_rentButtonBy);
                return !btn.Enabled || !btn.Displayed;
            }
            catch (NoSuchElementException) { return true; } // Si no está el botón, está "deshabilitado" visualmente
        }

        public bool CheckShoppingCart(string deviceName) {
            return _driver.PageSource.Contains(deviceName);
        }

        public bool CheckMessageError(string expectedError) {
            try {
                return _driver.FindElement(_errorAreaBy).Text.Contains(expectedError);
            }
            catch (NoSuchElementException) { return false; }
        }

        public bool CheckMessageErrorNotAvailableDevices(string error) {
            return _driver.PageSource.Contains(error);
        }
    }
}