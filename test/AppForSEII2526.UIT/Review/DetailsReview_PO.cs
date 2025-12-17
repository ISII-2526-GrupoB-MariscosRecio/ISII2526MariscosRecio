using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Review {
    public class DetailReview_PO : PageObject {
        public DetailReview_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) {
        }

        public bool CheckReviewDetails(string title, DateTime reviewDate, string customerName, string country) {
            // Esperamos a que el título sea visible para asegurar que la página cargó
            WaitForBeingVisible(By.Id("ReviewTitle"));

            bool result = true;

            // Verificamos los campos de texto simple
            result = result && _driver.FindElement(By.Id("ReviewTitle")).Text.Contains(title);
            result = result && _driver.FindElement(By.Id("CustomerName")).Text.Contains(customerName);

            // Verificamos el país (recuerda pasar el nombre "Spain", no el ID 1)
            result = result && _driver.FindElement(By.Id("Country")).Text.Contains(country);

            // Verificamos la fecha
            // En tu UI usas: .ToString("dd/MM/yyyy HH:mm:ss")
            var dateText = _driver.FindElement(By.Id("ReviewDate")).Text;

            // Parseamos la fecha del navegador para compararla
            // Usamos CultureInfo.InvariantCulture para evitar problemas con formatos regionales del servidor vs local
            DateTime actualDate = DateTime.ParseExact(dateText, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

            // Comparamos con un margen de error pequeño (por ejemplo 1 minuto) o igualdad exacta según prefieras.
            // Aquí sigo la lógica de tu ejemplo (TimeSpan < 1 min)
            result = result && (Math.Abs((actualDate - reviewDate).TotalMinutes) < 1);

            return result;
        }

        public bool CheckListOfDevices(List<string[]> expectedReviewItems) {
            // Mapeado al ID de tu tabla HTML: <table ... id="ReviewedDevices">
            return CheckBodyTable(expectedReviewItems, By.Id("ReviewedDevices"));
        }
    }
}