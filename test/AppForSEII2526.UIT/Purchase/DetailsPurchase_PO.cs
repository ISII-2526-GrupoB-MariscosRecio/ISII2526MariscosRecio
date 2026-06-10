using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using AppForSEII2526.UIT.Shared;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Purchase
{
    public class DetailsPurchase_PO : PageObject
    {
        // Selectores basados en DetailsDEVICEScompra.razor
        private By _nameField = By.Id("Name");
        private By _surnameField = By.Id("Surname");
        private By _addressField = By.Id("DeliveryAddress");
        private By _dateField = By.Id("PurchaseDate");
        private By _totalPriceField = By.Id("TotalPrice");
        private By _purchasedDevicesTable = By.Id("PurchasedDevices");
        private By _alertDanger = By.CssSelector(".alert-danger");
        private By _spinner = By.CssSelector(".spinner-border");

        public DetailsPurchase_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

        // Esperar a que cargue la página
        public void WaitForPageLoad()
        {
            System.Threading.Thread.Sleep(1000);
            try
            {
                // Esperar a que desaparezca el spinner
                var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                wait.Until(driver =>
                    driver.FindElements(_spinner).Count == 0);
            }
            catch
            {
                // Continuar si hay timeout
            }
        }

        // Paso 7: Verificar detalles de la compra realizada
        // Verifica: nombre, apellidos, dirección de entrega, fecha, precio total, cantidad total y dispositivos
        public bool CheckPurchaseDetail(string name, string surname, string address)
        {
            WaitForPageLoad();
            WaitForBeingVisible(_nameField);

            bool ok = true;

            try
            {
                // Verificar nombre
                var actualName = _driver.FindElement(_nameField).Text;
                if (!actualName.Contains(name))
                {
                    _output.WriteLine($"Error Name: expected '{name}', got '{actualName}'");
                    ok = false;
                }

                // Verificar apellido
                var actualSurname = _driver.FindElement(_surnameField).Text;
                if (!actualSurname.Contains(surname))
                {
                    _output.WriteLine($"Error Surname: expected '{surname}', got '{actualSurname}'");
                    ok = false;
                }

                // Verificar dirección
                var actualAddress = _driver.FindElement(_addressField).Text;
                if (!actualAddress.Contains(address))
                {
                    _output.WriteLine($"Error Address: expected '{address}', got '{actualAddress}'");
                    ok = false;
                }

                // Verificar que hay fecha (cualquier formato)
                var actualDate = _driver.FindElement(_dateField).Text;
                if (string.IsNullOrEmpty(actualDate))
                {
                    _output.WriteLine("Error: PurchaseDate is empty");
                    ok = false;
                }

                // Verificar que hay precio total
                var totalPrice = _driver.FindElement(_totalPriceField).Text;
                if (string.IsNullOrEmpty(totalPrice))
                {
                    _output.WriteLine("Error: TotalPrice is empty");
                    ok = false;
                }

                // Verificar que hay dispositivos en la tabla
                var tableRows = _driver.FindElement(_purchasedDevicesTable)
                    .FindElement(By.TagName("tbody"))
                    .FindElements(By.TagName("tr"));
                if (tableRows.Count == 0)
                {
                    _output.WriteLine("Error: No devices in purchase");
                    ok = false;
                }
            }
            catch (NoSuchElementException ex)
            {
                _output.WriteLine($"Element not found: {ex.Message}");
                ok = false;
            }

            return ok;
        }

        // Verificar tabla de dispositivos comprados
        public bool CheckListOfDevices(List<string[]> items)
        {
            return CheckBodyTable(items, _purchasedDevicesTable);
        }

        // Obtener precio total
        public string GetTotalPrice()
        {
            try
            {
                return _driver.FindElement(_totalPriceField).Text;
            }
            catch (NoSuchElementException)
            {
                return "0";
            }
        }

        // Verificar que se muestra error
        public bool CheckErrorDisplayed()
        {
            try
            {
                var alert = _driver.FindElement(_alertDanger);
                return !string.IsNullOrEmpty(alert.Text);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        // Obtener mensaje de error
        public string GetErrorMessage()
        {
            try
            {
                return _driver.FindElement(_alertDanger).Text;
            }
            catch (NoSuchElementException)
            {
                return string.Empty;
            }
        }

        // Verificar que la compra se cargó correctamente
        public bool IsPurchaseLoaded()
        {
            try
            {
                WaitForPageLoad();
                return _driver.FindElements(_nameField).Count > 0 &&
                       _driver.FindElements(_alertDanger).Count == 0;
            }
            catch
            {
                return false;
            }
        }

        // Obtener nombre del cliente
        public string GetCustomerName()
        {
            try
            {
                return _driver.FindElement(_nameField).Text;
            }
            catch (NoSuchElementException)
            {
                return string.Empty;
            }
        }

        // Obtener apellido del cliente
        public string GetCustomerSurname()
        {
            try
            {
                return _driver.FindElement(_surnameField).Text;
            }
            catch (NoSuchElementException)
            {
                return string.Empty;
            }
        }

        // Obtener dirección de entrega
        public string GetDeliveryAddress()
        {
            try
            {
                return _driver.FindElement(_addressField).Text;
            }
            catch (NoSuchElementException)
            {
                return string.Empty;
            }
        }

        // Obtener fecha de compra
        public string GetPurchaseDate()
        {
            try
            {
                return _driver.FindElement(_dateField).Text;
            }
            catch (NoSuchElementException)
            {
                return string.Empty;
            }
        }

        // Obtener cantidad de dispositivos comprados
        public int GetPurchasedDevicesCount()
        {
            try
            {
                var tableRows = _driver.FindElement(_purchasedDevicesTable)
                    .FindElement(By.TagName("tbody"))
                    .FindElements(By.TagName("tr"));
                return tableRows.Count;
            }
            catch (NoSuchElementException)
            {
                return 0;
            }
        }

        // Verificar que un dispositivo específico está en la lista de comprados
        public bool ContainsDevice(string deviceName)
        {
            try
            {
                var table = _driver.FindElement(_purchasedDevicesTable);
                return table.Text.Contains(deviceName);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        // Verificar detalles completos según el paso 7 del caso de uso
        public bool CheckFullPurchaseDetails(string name, string surname, string address,
            int expectedDeviceCount = 1, string expectedDeviceName = null)
        {
            bool ok = CheckPurchaseDetail(name, surname, address);

            // Verificar cantidad de dispositivos si se especifica
            if (expectedDeviceCount > 0)
            {
                int actualCount = GetPurchasedDevicesCount();
                if (actualCount < expectedDeviceCount)
                {
                    _output.WriteLine($"Error: Expected at least {expectedDeviceCount} devices, got {actualCount}");
                    ok = false;
                }
            }

            // Verificar dispositivo específico si se especifica
            if (!string.IsNullOrEmpty(expectedDeviceName))
            {
                if (!ContainsDevice(expectedDeviceName))
                {
                    _output.WriteLine($"Error: Device '{expectedDeviceName}' not found in purchase");
                    ok = false;
                }
            }

            return ok;
        }
    }
}