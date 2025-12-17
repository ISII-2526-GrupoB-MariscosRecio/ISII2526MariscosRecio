using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using AppForSEII2526.UIT.Shared;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Purchase // Namespace Purchase
{
    public class SelectDevice_PO : PageObject
    {
        // SELECTORES BASADOS EN TU SelectDEVICEScompra.razor

        // Filtros (Tú usas IDs explícitos, ¡bien hecho!)
        private By _inputNameBy = By.Id("filterName");
        private By _inputColorBy = By.Id("filterColor");

        // Botón limpiar filtros
        private By _clearFiltersBtn = By.XPath("//button[contains(text(), 'Limpiar Filtros')]");

        // Botón Tramitar (verde)
        private By _tramitarButtonBy = By.XPath("//button[contains(text(), 'Tramitar')]");

        // Alertas
        private By _alertWarning = By.CssSelector(".alert-warning");

        public SelectDevice_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

        public void Visit()
        {
            _driver.Navigate().GoToUrl(URI + "/compra/seleccion");
        }

        // Adaptado: Tú filtras por Nombre y Color, no por Fechas/Precio
        public void FilterDevices(string name, string color)
        {
            WaitForBeingVisible(_inputNameBy);

            if (!string.IsNullOrEmpty(name))
            {
                var input = _driver.FindElement(_inputNameBy);
                input.Clear();
                input.SendKeys(name);
                // Pausa técnica porque Blazor filtra "oninput" (en tiempo real)
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

        public void SelectDevice(string deviceName)
        {
            // Busca la tarjeta que tenga ese nombre y pulsa el botón "Añadir"
            // Tu botón: <button class="btn btn-primary w-100" ...> <i class="bi bi-cart-plus"></i> Añadir </button>
            // XPath: Busca div con texto del nombre, luego baja al botón
            string xpathAddBtn = $"//div[contains(@class,'card') and contains(., '{deviceName}')]//button[contains(., 'Añadir')]";

            WaitForBeingVisible(By.XPath(xpathAddBtn));
            _driver.FindElement(By.XPath(xpathAddBtn)).Click();

            // Espera visual para ver que el contador del carrito sube
            System.Threading.Thread.Sleep(500);
        }

        public void ClickTramitar()
        {
            WaitForBeingClickable(_tramitarButtonBy);
            _driver.FindElement(_tramitarButtonBy).Click();
        }

        public bool CheckRentButtonDisabledOrMissing()
        {
            // En tu código: @if (PurchaseState...Any()) { mostrar botón }
            // Si el carrito está vacío, el botón NO existe en el DOM.
            return _driver.FindElements(_tramitarButtonBy).Count == 0;
        }

        public bool CheckMessageErrorNotAvailable(string expectedText)
        {
            // Verifica la alerta amarilla de "No se encontraron dispositivos"
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

        public bool IsDeviceVisible(string deviceName)
        {
            return _driver.PageSource.Contains(deviceName);
        }
    }
}