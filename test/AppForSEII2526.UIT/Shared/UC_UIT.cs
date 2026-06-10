using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using System;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Shared {
    public class UC_UIT : IDisposable {

        private bool _pipeline = false;
        private string _browser = "Edge";
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
            _driver.Navigate()
                    .GoToUrl(_URI + "Account/Login");
            
            // Esperar a que cargue la página de login
            System.Threading.Thread.Sleep(1000);
            
            // Intentar múltiples selectores para el campo de email
            IWebElement emailField = null;
            try {
                // Intento 1: Por nombre con punto
                emailField = _driver.FindElement(By.Name("Input.Email"));
            } catch (NoSuchElementException) {
                try {
                    // Intento 2: Por ID
                    emailField = _driver.FindElement(By.Id("Input_Email"));
                } catch (NoSuchElementException) {
                    try {
                        // Intento 3: Por tipo email
                        emailField = _driver.FindElement(By.CssSelector("input[type='email']"));
                    } catch (NoSuchElementException) {
                        // Intento 4: Por autocomplete
                        emailField = _driver.FindElement(By.CssSelector("input[autocomplete='username']"));
                    }
                }
            }
            
            emailField.Clear();
            emailField.SendKeys(email);

            // Intentar múltiples selectores para el campo de password
            IWebElement passwordField = null;
            try {
                passwordField = _driver.FindElement(By.Name("Input.Password"));
            } catch (NoSuchElementException) {
                try {
                    passwordField = _driver.FindElement(By.Id("Input_Password"));
                } catch (NoSuchElementException) {
                    passwordField = _driver.FindElement(By.CssSelector("input[type='password']"));
                }
            }
            
            passwordField.Clear();
            passwordField.SendKeys(password);

            // Buscar el botón de login
            IWebElement loginButton = null;
            try {
                // Intento 1: XPath original
                loginButton = _driver.FindElement(By.XPath("/html/body/div[1]/main/article/div/div[1]/section/form/div[4]/button"));
            } catch (NoSuchElementException) {
                try {
                    // Intento 2: Por tipo submit
                    loginButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
                } catch (NoSuchElementException) {
                    // Intento 3: Por texto
                    loginButton = _driver.FindElement(By.XPath("//button[contains(text(), 'Log in') or contains(text(), 'Iniciar') or contains(text(), 'Login')]"));
                }
            }
            
            loginButton.Click();
            
            // Esperar a que se complete el login
            System.Threading.Thread.Sleep(2000);
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