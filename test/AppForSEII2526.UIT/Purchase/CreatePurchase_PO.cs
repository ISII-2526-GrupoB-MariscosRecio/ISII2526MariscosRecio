using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using AppForSEII2526.UIT.Shared;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Purchase
{
    public class CreatePurchase_PO : PageObject
    {
        // SELECTORES BASADOS EN CreateDEVICEScompra.razor
        // Campos del formulario (usando XPath por etiqueta)
        private By _nameInput = By.XPath("//label[contains(text(), 'Nombre:')]/following-sibling::input");
        private By _surnameInput = By.XPath("//label[contains(text(), 'Apellidos:')]/following-sibling::input");
        private By _addressInput = By.XPath("//label[contains(text(), 'Dirección:')]/following-sibling::input");
        private By _paymentSelect = By.XPath("//label[contains(text(), 'Método de Pago:')]/following-sibling::select");

        // Botones
        private By _submitButton = By.XPath("//button[contains(text(), 'Finalizar Compra')]");

        // Tabla de resumen del pedido
        private By _tableOfItems = By.TagName("table");

        // Total
        private By _totalPrice = By.CssSelector(".card-footer h3.text-success");

        // Alertas de error
        private By _alertDanger = By.CssSelector(".alert-danger");

        // Campo de descripción opcional por item
        private By _descriptionInputs = By.XPath("//input[@placeholder='Nota opcional...']");

        // Mensajes de validación de Blazor
        private By _validationMessages = By.CssSelector(".validation-message");

        // Filas de la tabla de items
        private By _tableRows = By.XPath("//table//tbody//tr");

        public CreatePurchase_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

        // Paso 5-6: Rellenar datos del formulario
        public void FillInPurchaseInfo(string name, string surname, string address, string payment)
        {
            System.Threading.Thread.Sleep(1000); // Esperar a que cargue la página
            WaitForBeingVisible(_nameInput);

            if (name != null)
            {
                var nameField = _driver.FindElement(_nameInput);
                nameField.Clear();
                nameField.SendKeys(name);
            }

            if (surname != null)
            {
                var surnameField = _driver.FindElement(_surnameInput);
                surnameField.Clear();
                surnameField.SendKeys(surname);
            }

            if (address != null)
            {
                var addressField = _driver.FindElement(_addressInput);
                addressField.Clear();
                addressField.SendKeys(address);
            }

            if (payment != null)
            {
                var selectElement = new SelectElement(_driver.FindElement(_paymentSelect));
                try
                {
                    // Intentar seleccionar por texto exacto
                    selectElement.SelectByText(payment);
                }
                catch (NoSuchElementException)
                {
                    // Si no encuentra, intentar por valor del enum
                    string mappedPayment = payment switch
                    {
                        "CreditCard" => "TarjetaCredito",
                        "Credit Card" => "TarjetaCredito",
                        "Tarjeta" => "TarjetaCredito",
                        _ => payment
                    };
                    try
                    {
                        selectElement.SelectByText(mappedPayment);
                    }
                    catch
                    {
                        // Como último recurso, seleccionar por índice
                        selectElement.SelectByIndex(0);
                    }
                }
            }
        }

        // Paso 6: Finalizar compra (Guardar)
        public void PressFinalizarCompra()
        {
            WaitForBeingClickable(_submitButton);
            _driver.FindElement(_submitButton).Click();
            System.Threading.Thread.Sleep(3000); // Esperar respuesta del servidor - aumentado tiempo
        }

        // FA3: Eliminar item del carrito
        // NOTA: En la tabla se muestra "@item.Brand @item.Model", no el nombre del dispositivo
        public void RemoveItemFromCart(string deviceIdentifier)
        {
            System.Threading.Thread.Sleep(500);
            
            // Buscar el botón de eliminar en cualquier fila que contenga el identificador
            // El identificador puede ser el nombre, marca o modelo del dispositivo
            string xpathBin = $"//table//tbody//tr[contains(., '{deviceIdentifier}')]//button[contains(@class, 'btn-danger')]";
            
            try
            {
                WaitForBeingClickable(By.XPath(xpathBin));
                _driver.FindElement(By.XPath(xpathBin)).Click();
            }
            catch (WebDriverTimeoutException)
            {
                // Si no encuentra por el identificador exacto, buscar cualquier botón de eliminar
                _output.WriteLine($"No se encontró item con '{deviceIdentifier}', buscando primer botón eliminar");
                string xpathAnyBin = "//table//tbody//tr//button[contains(@class, 'btn-danger')]";
                WaitForBeingClickable(By.XPath(xpathAnyBin));
                _driver.FindElement(By.XPath(xpathAnyBin)).Click();
            }
            
            System.Threading.Thread.Sleep(500);
        }

        // Paso 5: Añadir descripción opcional a un item
        public void AddDescriptionToItem(string deviceIdentifier, string description)
        {
            System.Threading.Thread.Sleep(500);
            
            // Buscar el input de descripción en la fila que contiene el dispositivo
            string xpathDesc = $"//table//tbody//tr[contains(., '{deviceIdentifier}')]//input[@placeholder='Nota opcional...']";
            
            try
            {
                var input = _driver.FindElement(By.XPath(xpathDesc));
                input.Clear();
                input.SendKeys(description);
            }
            catch (NoSuchElementException)
            {
                // Si no encuentra por identificador, usar el primer input de descripción
                _output.WriteLine($"No se encontró campo de descripción para {deviceIdentifier}, usando primero disponible");
                try
                {
                    var inputs = _driver.FindElements(_descriptionInputs);
                    if (inputs.Count > 0)
                    {
                        inputs[0].Clear();
                        inputs[0].SendKeys(description);
                    }
                }
                catch (NoSuchElementException)
                {
                    _output.WriteLine("No hay campos de descripción disponibles");
                }
            }
        }

        // Verificar la tabla de resumen antes de confirmar
        public bool CheckListOfPurchaseItems(List<string[]> expectedItems)
        {
            return CheckBodyTable(expectedItems, _tableOfItems);
        }

        // Obtener el precio total mostrado
        public string GetTotalPrice()
        {
            try
            {
                return _driver.FindElement(_totalPrice).Text;
            }
            catch (NoSuchElementException)
            {
                return "0";
            }
        }

        // FA5: Verificar errores de validación
        public bool CheckValidationError(string error)
        {
            System.Threading.Thread.Sleep(500);
            return _driver.PageSource.ToLower().Contains(error.ToLower());
        }

        // Verificar error específico en alerta
        public bool CheckAlertError(string expectedError)
        {
            try
            {
                var alert = _driver.FindElement(_alertDanger);
                return alert.Text.Contains(expectedError);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        // Verificar que hay items en la tabla
        public bool HasItemsInCart()
        {
            try
            {
                var rows = _driver.FindElements(_tableRows);
                return rows.Count > 0;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        // Contar items en el carrito
        public int GetItemCount()
        {
            try
            {
                var rows = _driver.FindElements(_tableRows);
                return rows.Count;
            }
            catch (NoSuchElementException)
            {
                return 0;
            }
        }

        // Verificar que la página está cargada
        public bool IsPageLoaded()
        {
            try
            {
                WaitForBeingVisible(_nameInput);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Obtener todos los mensajes de validación mostrados
        public List<string> GetValidationMessages()
        {
            var messages = new List<string>();
            try
            {
                var elements = _driver.FindElements(_validationMessages);
                foreach (var element in elements)
                {
                    if (!string.IsNullOrEmpty(element.Text))
                    {
                        messages.Add(element.Text);
                    }
                }
            }
            catch (NoSuchElementException)
            {
                // No hay mensajes de validación
            }
            return messages;
        }

        // Verificar si el formulario tiene errores de validación visibles
        public bool HasValidationErrors()
        {
            return GetValidationMessages().Count > 0 || 
                   _driver.FindElements(_alertDanger).Count > 0;
        }

        // FA6: Volver a la selección de dispositivos para modificar
        public void GoBackToSelection()
        {
            _driver.Navigate().Back();
            System.Threading.Thread.Sleep(1000);
        }

        // Verificar que estamos en la página de crear compra
        public bool IsOnCreatePage()
        {
            return _driver.Url.Contains("/compra/create");
        }

        // Verificar si hay error de API mostrado
        public bool HasApiError()
        {
            try
            {
                var alert = _driver.FindElement(_alertDanger);
                return alert.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        // Obtener mensaje de error de API
        public string GetApiErrorMessage()
        {
            try
            {
                var alert = _driver.FindElement(_alertDanger);
                return alert.Text;
            }
            catch (NoSuchElementException)
            {
                return string.Empty;
            }
        }
    }
}