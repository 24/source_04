using OpenQA.Selenium;

namespace pb.Web.WebDriver
{
    public static class WebDriverManager
    {
        private static IWebDriver __currentWebDriver = null;

        public static IWebDriver CurrentWebDriver { get { return __currentWebDriver; } }

        public static void SetCurrentWebDriver(IWebDriver webDriver)
        {
            __currentWebDriver = webDriver;
        }

        //public WebDriverManager()
        //{
        //    _chromeDriverService = null;
        //}
        //public abstract IWebDriver CreateWebDriver();
    }
}
