using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace pb.Web.WebDriver
{
    public class FirefoxWebDriverManager
    {
        //private FirefoxDriverService _driverService = null;
        ////private FirefoxOptions _options = null;

        //public FirefoxWebDriverManager(string driverDirectory = null)
        //{
        //    if (driverDirectory != null)
        //        _driverService = FirefoxDriverService.CreateDefaultService(driverDirectory);
        //    else
        //        _driverService = FirefoxDriverService.CreateDefaultService();
        //    //_options = new FirefoxOptions();
        //}

        public static IWebDriver CreateWebDriver(string driverDirectory = null)
        {
            return new FirefoxDriver(CreateDriverService(driverDirectory));
        }

        public static FirefoxDriverService CreateDriverService(string driverDirectory = null)
        {
            FirefoxDriverService service;
            if (driverDirectory != null)
                service = FirefoxDriverService.CreateDefaultService(driverDirectory);
            else
                service = FirefoxDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            service.SuppressInitialDiagnosticInformation = true;
            return service;
        }
    }
}
