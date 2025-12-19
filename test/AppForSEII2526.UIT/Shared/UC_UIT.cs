using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using System;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Shared {
    public class UC_UIT : IDisposable {

        private bool _pipeline = false;
        private string _browser = "Chrome";
        protected IWebDriver _driver;
        protected readonly ITestOutputHelper _output;

        public string _URI {
            get {
                // Asegúrate de que este puerto coincida con tu launchSettings.json (https)
                return "https://localhost:7081/";
            }
        }

        public UC_UIT(ITestOutputHelper output) {
            _output = output;
            switch (_browser) {
                case "Firefox": SetUp_FireFox4UIT(); break;
                case "Edge": SetUp_EdgeFor4UIT(); break;
                default: SetUp_Chrome4UIT(); break;
            }
            _driver.Manage().Window.Maximize();
        }

        protected void Initial_step_opening_the_web_page() {
            _driver.Navigate().GoToUrl(_URI);
        }

        protected void Perform_login(string email, string password) {
            _driver.Navigate().GoToUrl(_URI + "Account/Login");

            // Usamos selectores más robustos que el XPath posicional
            // InputText de Blazor genera name="Model.Property"
            _driver.FindElement(By.Name("Input.Email")).SendKeys(email);
            _driver.FindElement(By.Name("Input.Password")).SendKeys(password);

            // Selector CSS para el botón de submit (funciona siempre que sea <button type="submit">)
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        }

        protected void SetUp_Chrome4UIT() {
            var optionsc = new ChromeOptions {
                PageLoadStrategy = PageLoadStrategy.Normal,
                AcceptInsecureCertificates = true
            };
            if (_pipeline) optionsc.AddArgument("--headless");
            _driver = new ChromeDriver(optionsc);
        }

        protected void SetUp_FireFox4UIT() {
            var optionsff = new FirefoxOptions {
                PageLoadStrategy = PageLoadStrategy.Normal,
                AcceptInsecureCertificates = true
            };
            if (_pipeline) optionsff.AddArgument("--headless");
            _driver = new FirefoxDriver(optionsff);
        }

        protected void SetUp_EdgeFor4UIT() {
            var optionsEdge = new EdgeOptions {
                PageLoadStrategy = PageLoadStrategy.Normal,
                AcceptInsecureCertificates = true
            };
            if (_pipeline) optionsEdge.AddArgument("--headless");
            _driver = new EdgeDriver(optionsEdge);
        }

        public void Dispose() {
            try {
                _driver.Close();
                _driver.Dispose();
            }
            catch { }
            GC.SuppressFinalize(this);
        }
    }
}