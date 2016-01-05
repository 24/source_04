using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace pb.Web.WebDriver
{
    public static class ChromeWebDriverManager
    {
        //private ChromeDriverService _driverService = null;
        //private ChromeOptions _options = null;

        //public ChromeWebDriverManager(string driverDirectory = null)
        //{
        //    if (driverDirectory != null)
        //        _driverService = ChromeDriverService.CreateDefaultService(driverDirectory);
        //    else
        //        _driverService = ChromeDriverService.CreateDefaultService();
        //    _driverService.HideCommandPromptWindow = true;
        //    _driverService.SuppressInitialDiagnosticInformation = true;
        //    _driverService.Start();
        //    _options = new ChromeOptions();
        //    _options.LeaveBrowserRunning = true;
        //}

        public static IWebDriver CreateWebDriver(string driverDirectory = null)
        {
            ChromeDriverService service = CreateDriverService(driverDirectory);
            // execute "chromedriver.exe"
            service.Start();
            return new ChromeDriver(service, CreateOptions());
        }

        public static ChromeDriverService CreateDriverService(string driverDirectory = null)
        {
            ChromeDriverService service;
            if (driverDirectory != null)
                service = ChromeDriverService.CreateDefaultService(driverDirectory);
            else
                service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            service.SuppressInitialDiagnosticInformation = true;
            return service;
        }

        public static ChromeOptions CreateOptions()
        {
            ChromeOptions options = new ChromeOptions();
            options.LeaveBrowserRunning = true;
            return options;
        }
    }
}
