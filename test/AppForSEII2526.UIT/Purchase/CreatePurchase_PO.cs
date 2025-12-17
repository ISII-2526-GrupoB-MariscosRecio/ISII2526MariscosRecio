using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using AppForSEII2526.UIT.Shared;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Purchase // Cambiado namespace a Purchase
{
    public class CreatePurchase_PO : PageObject
    {
        // SELECTORES BASADOS EN TU CreateDEVICEScompra.razor
        // Como usas <InputText> sin ID, usamos XPath buscando por la etiqueta Label anterior
        private By _nameInput = By.XPath("//label[contains(text(), 'Nombre:')]/following-sibling::input");
        private By _surnameInput = By.XPath("//label[contains(text(), 'Apellidos:')]/following-sibling::input");
        private By _addressInput = By.XPath("//label[contains(text(), 'Dirección:')]/following-sibling::input");
        private By _paymentSelect = By.XPath("//label[contains(text(), 'Método de Pago:')]/following-sibling::select");

        // Botones
        private By _submitButton = By.XPath("//button[contains(text(), 'Finalizar Compra')]");
        private By _backButton = By.XPath("//button[contains(text(), 'Seguir Comprando') or contains(text(), 'Volver')]");
        // (Nota: Si no pusiste el botón de volver en el sprint anterior, este selector fallará si se llama)

        private By _tableOfItems = By.TagName("table"); // La tabla de resumen del pedido

        public CreatePurchase_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

        public void FillInPurchaseInfo(string name, string surname, string address, string payment)
        {
            // Esperamos que cargue el primer campo
            WaitForBeingVisible(_nameInput);

            if (name != null)
            {
                _driver.FindElement(_nameInput).Clear();
                _driver.FindElement(_nameInput).SendKeys(name);
            }

            if (surname != null)
            {
                _driver.FindElement(_surnameInput).Clear();
                _driver.FindElement(_surnameInput).SendKeys(surname);
            }

            if (address != null)
            {
                _driver.FindElement(_addressInput).Clear();
                _driver.FindElement(_addressInput).SendKeys(address);
            }

            if (payment != null)
            {
                // Seleccionar método de pago si se proporciona
                new SelectElement(_driver.FindElement(_paymentSelect)).SelectByText(payment);
            }
        }

        public void PressFinalizarCompra()
        {
            WaitForBeingClickable(_submitButton);
            _driver.FindElement(_submitButton).Click();
        }

        public void RemoveItemFromCart(string deviceModel)
        {
            // Busca el botón de eliminar (basura) en la fila que contiene el modelo
            // Tu código usa <i class="bi bi-trash"></i> dentro de un botón danger
            string xpathBin = $"//tr[contains(., '{deviceModel}')]//button[contains(@class, 'btn-danger')]";
            WaitForBeingVisible(By.XPath(xpathBin));
            _driver.FindElement(By.XPath(xpathBin)).Click();
        }

        // Verifica la tabla de resumen antes de confirmar (lo que pedía tu compañero)
        public bool CheckListOfPurchaseItems(List<string[]> expectedItems)
        {
            return CheckBodyTable(expectedItems, _tableOfItems);
        }

        public bool CheckValidationError(string error)
        {
            // Blazor usa la clase .validation-message para los errores bajo los inputs
            // O busca en todo el código fuente si es un error general
            return _driver.PageSource.Contains(error);
        }
    }
}