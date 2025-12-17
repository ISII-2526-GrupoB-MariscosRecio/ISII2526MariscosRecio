using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using AppForSEII2526.UIT.Shared;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Purchase // Namespace Purchase
{
    public class DetailsPurchase_PO : PageObject
    {
        public DetailsPurchase_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

        // Adaptado a tus IDs de DetailsDEVICEScompra.razor
        public bool CheckPurchaseDetail(string name, string surname, string address)
        {
            // Esperamos a que cargue la tabla de detalles
            WaitForBeingVisible(By.Id("Name"));

            bool ok = true;

            // Verificamos campos usando los IDs que pusiste en el Razor
            ok &= _driver.FindElement(By.Id("Name")).Text.Contains(name);
            ok &= _driver.FindElement(By.Id("Surname")).Text.Contains(surname);
            ok &= _driver.FindElement(By.Id("DeliveryAddress")).Text.Contains(address);

            // Verificamos que hay una fecha (cualquier formato)
            ok &= _driver.FindElement(By.Id("PurchaseDate")).Text.Length > 0;

            return ok;
        }

        // Verifica la tabla inferior de dispositivos comprados
        public bool CheckListOfDevices(List<string[]> items)
        {
            // Tu tabla tiene id="PurchasedDevices"
            return CheckBodyTable(items, By.Id("PurchasedDevices"));
        }

        public string GetTotalPrice()
        {
            return _driver.FindElement(By.Id("TotalPrice")).Text;
        }
    }
}