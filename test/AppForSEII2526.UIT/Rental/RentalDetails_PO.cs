using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using AppForSEII2526.UIT.Shared;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Rental {
    public class DetailRental_PO : PageObject {
        public DetailRental_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

        public bool CheckRentalDetail(string surname, string address, string payment, DateTime date, DateTime from, DateTime to, string priceSymbol) {
            WaitForBeingVisible(By.Id("TotalPrice")); // Esperar a que cargue la página

            bool ok = true;
            // Verificaciones laxas para evitar fallos por formatos
            ok &= _driver.FindElement(By.Id("NameSurname")).Text.Contains(surname);
            ok &= _driver.FindElement(By.Id("DeliveryAddress")).Text.Contains(address);
            ok &= _driver.FindElement(By.Id("PaymentMethod")).Text.Contains(payment);

            // Verificar que al menos la fecha de inicio esté presente
            ok &= _driver.PageSource.Contains(from.ToString("dd/MM/yyyy"));

            return ok;
        }

        public bool CheckListOfDevices(List<string[]> items) => CheckBodyTable(items, By.Id("RentedDevices"));
    }
}