using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using AppForSEII2526.UIT.Shared;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Purchase
{
    public class SelectDevice_PO : PageObject
    {
        // SELECTORES BASADOS EN TU SelectDEVICEScompra.razor

        // Filtros (Tú usas IDs explícitos)
        private By _inputNameBy = By.Id("filterName");
        private By _inputColorBy = By.Id("filterColor");

        // Botón limpiar filtros
        private By _clearFiltersBtn = By.XPath("//button[contains(text(), 'Limpiar Filtros')]");

        // Botón Tramitar (verde)
        private By _tramitarButtonBy = By.XPath("//button[contains(text(), 'Tramitar')]");

        // Alertas
        private By _alertWarning = By.CssSelector(".alert-warning");

        // Spinner de carga
        private By _spinner = By.CssSelector(".spinner-border");

        // Contenedor de tarjetas de dispositivos
        private By _deviceCards = By.CssSelector(".card.h-100");

        // Total del carrito
        private By _cartTotal = By.CssSelector(".card.border-success h3");
        private By _cartItemCount = By.CssSelector(".card.border-success .small");

        public SelectDevice_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

        public void Visit(string baseUri)
        {
            _driver.Navigate().GoToUrl(baseUri + "compra/seleccion");
            // Esperar a que cargue la página (desaparezca el spinner o aparezcan dispositivos)
            WaitForPageLoad();
        }

        private void WaitForPageLoad()
        {
            // Espera hasta que el spinner desaparezca o aparezcan dispositivos/alerta
            System.Threading.Thread.Sleep(1000);
            try
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                wait.Until(driver =>
                    driver.FindElements(_spinner).Count == 0 ||
                    driver.FindElements(_deviceCards).Count > 0 ||
                    driver.FindElements(_alertWarning).Count > 0);
            }
            catch (WebDriverTimeoutException)
            {
                // Si hay timeout, continuamos
            }
        }

        // FA1: Filtrar dispositivos por nombre y/o color
        public void FilterDevices(string name, string color)
        {
            WaitForBeingVisible(_inputNameBy);

            if (!string.IsNullOrEmpty(name))
            {
                var input = _driver.FindElement(_inputNameBy);
                input.Clear();
                input.SendKeys(name);
                System.Threading.Thread.Sleep(500);
            }

            if (!string.IsNullOrEmpty(color))
            {
                var input = _driver.FindElement(_inputColorBy);
                input.Clear();
                input.SendKeys(color);
                System.Threading.Thread.Sleep(500);
            }
        }

        // Limpiar filtros
        public void ClearFilters()
        {
            WaitForBeingClickable(_clearFiltersBtn);
            _driver.FindElement(_clearFiltersBtn).Click();
            System.Threading.Thread.Sleep(500);
        }

        // Paso 3: Seleccionar dispositivo y añadir al carrito
        // NOTA: En el Razor se muestra "@device.Brand @device.Name", por lo que deviceName puede ser
        // el nombre completo (ej: "Apple iPhone 11") o parcial (ej: "iPhone 11")
        public void SelectDevice(string deviceName)
        {
            // Primero intentamos buscar por el nombre exacto en el card-title
            string xpathAddBtn = $"//div[contains(@class,'card')]//h5[contains(@class,'card-title') and contains(., '{deviceName}')]/ancestor::div[contains(@class,'card')]//button[contains(., 'Añadir')]";
            
            try
            {
                WaitForBeingVisible(By.XPath(xpathAddBtn));
                _driver.FindElement(By.XPath(xpathAddBtn)).Click();
            }
            catch (NoSuchElementException)
            {
                // Si no lo encuentra, buscar en cualquier parte del card
                string xpathFallback = $"//div[contains(@class,'card') and contains(., '{deviceName}')]//button[contains(., 'Añadir')]";
                WaitForBeingVisible(By.XPath(xpathFallback));
                _driver.FindElement(By.XPath(xpathFallback)).Click();
            }

            System.Threading.Thread.Sleep(500);
        }

        // Paso 4: Click en Tramitar para ir a crear compra
        public void ClickTramitar()
        {
            WaitForBeingClickable(_tramitarButtonBy);
            _driver.FindElement(_tramitarButtonBy).Click();
            System.Threading.Thread.Sleep(1000);
        }

        // FA4: Verificar que el botón Tramitar no está disponible (carrito vacío)
        public bool CheckTramitarButtonDisabledOrMissing()
        {
            return _driver.FindElements(_tramitarButtonBy).Count == 0;
        }

        // Alias para compatibilidad
        public bool CheckRentButtonDisabledOrMissing()
        {
            return CheckTramitarButtonDisabledOrMissing();
        }

        // FA0: Verificar mensaje de no hay dispositivos disponibles
        public bool CheckNoDevicesAvailableMessage()
        {
            try
            {
                var alert = _driver.FindElement(_alertWarning);
                return alert.Text.Contains("No se encontraron");
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        // FA1: Verificar mensaje de filtros sin resultados
        public bool CheckMessageErrorNotAvailable(string expectedText)
        {
            try
            {
                var alert = _driver.FindElement(_alertWarning);
                return alert.Text.Contains(expectedText);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        // Verificar si un dispositivo es visible en la lista
        public bool IsDeviceVisible(string deviceName)
        {
            return _driver.PageSource.Contains(deviceName);
        }

        // Obtener cantidad de dispositivos mostrados
        public int GetVisibleDeviceCount()
        {
            return _driver.FindElements(_deviceCards).Count;
        }

        // Verificar el total del carrito
        public string GetCartTotal()
        {
            try
            {
                return _driver.FindElement(_cartTotal).Text;
            }
            catch (NoSuchElementException)
            {
                return "0";
            }
        }

        // Verificar cantidad de artículos en el carrito
        public string GetCartItemCount()
        {
            try
            {
                return _driver.FindElement(_cartItemCount).Text;
            }
            catch (NoSuchElementException)
            {
                return "0 artículos";
            }
        }

        // Verificar que el carrito tiene items (para FA4)
        public bool HasItemsInCart()
        {
            try
            {
                var countText = GetCartItemCount();
                // El texto es algo como "2 artículos"
                var parts = countText.Split(' ');
                if (int.TryParse(parts[0], out int count))
                {
                    return count > 0;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Obtener número de artículos en el carrito como entero
        public int GetCartItemCountAsInt()
        {
            try
            {
                var countText = GetCartItemCount();
                var parts = countText.Split(' ');
                if (int.TryParse(parts[0], out int count))
                {
                    return count;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}