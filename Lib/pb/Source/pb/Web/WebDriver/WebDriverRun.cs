using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace pb.Web.WebDriver
{
    public static class WebDriverRun
    {
        private static string __driverDirectory = null;
        private static IWebDriver __webDriver = null;
        private static IWebElement __webElement = null;
        private static ReadOnlyCollection<IWebElement> __webElements = null;

        // WebDriver properties
        public static string CurrentWindowHandle { get { return __webDriver != null ? __webDriver.CurrentWindowHandle : null; } }
        public static string PageSource { get { return __webDriver != null ? __webDriver.PageSource : null; } }
        public static string Title { get { return __webDriver != null ? __webDriver.Title : null; } }
        public static string Url { get { return __webDriver != null ? __webDriver.Url : null; } set { if (__webDriver != null) __webDriver.Url = value; } }
        public static ReadOnlyCollection<string> WindowHandles { get { return __webDriver != null ? __webDriver.WindowHandles : null; } }

        public static IWebElement Element { get { return __webElement; } set { __webElement = value; } }
        // WebElement properties
        //public static bool? ElementDisplayed { get { return __webElement != null ? (bool?)__webElement.Displayed : null; } }
        //public static bool? ElementEnabled { get { return __webElement != null ? (bool?)__webElement.Enabled : null; } }
        //public static Point? ElementLocation { get { return __webElement != null ? (Point?)__webElement.Location : null; } }
        //public static bool? ElementSelected { get { return __webElement != null ? (bool?)__webElement.Selected : null; } }
        //public static Size? ElementSize { get { return __webElement != null ? (Size?)__webElement.Size : null; } }
        //public static string ElementTagName { get { return __webElement != null ? __webElement.TagName : null; } }
        //public static string ElementText { get { return __webElement != null ? __webElement.Text : null; } }

        public static void SetDriverDirectory(string driverDirectory)
        {
            __driverDirectory = driverDirectory;
        }

        public static void CreateChromeWebDriver()
        {
            Quit();
            __webDriver = ChromeWebDriverManager.CreateWebDriver(__driverDirectory);
        }

        public static void CreateFirefoxWebDriver()
        {
            Quit();
            __webDriver = FirefoxWebDriverManager.CreateWebDriver(__driverDirectory);
        }

        // WebDriver method
        public static void Quit()
        {
            if (__webDriver != null)
            {
                __webDriver.Quit();
                __webDriver = null;
            }
        }

        public static void Close()
        {
            if (__webDriver != null)
                __webDriver.Close();
        }

        public static IOptions Manage()
        {
            if (__webDriver != null)
                return __webDriver.Manage();
            else
                return null;
        }

        public static INavigation Navigate()
        {
            if (__webDriver != null)
                return __webDriver.Navigate();
            else
                return null;
        }

        public static ITargetLocator SwitchTo()
        {
            if (__webDriver != null)
                return __webDriver.SwitchTo();
            else
                return null;
        }

        public static IWebElement FindElement(By by)
        {
            __webElement = __webDriver.FindElement(by);
            return __webElement;
        }

        public static ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            __webElements = __webDriver.FindElements(by);
            return __webElements;
        }

        // WebElement method
        //public static void ElementClear()
        //{
        //    if (__webElement != null)
        //        __webElement.Clear();
        //}

        //public static void ElementClick()
        //{
        //    if (__webElement != null)
        //        __webElement.Click();
        //}

        //public static string ElementGetAttribute(string attributeName)
        //{
        //    if (__webElement != null)
        //        return __webElement.GetAttribute(attributeName);
        //    else
        //        return null;
        //}

        //public static string ElementGetCssValue(string propertyName)
        //{
        //    if (__webElement != null)
        //        return __webElement.GetCssValue(propertyName);
        //    else
        //        return null;
        //}

        //public static void ElementSendKeys(string text)
        //{
        //    if (__webElement != null)
        //        __webElement.SendKeys(text);
        //}

        //public static void ElementSubmit()
        //{
        //    if (__webElement != null)
        //        __webElement.Submit();
        //}
    }

    public static class WebDriverRunExtension
    {
        public static IWebElement zFindElement(this IWebElement element, By by)
        {
            if (element == null)
                return null;
            WebDriverRun.Element = element.FindElement(by);
            return WebDriverRun.Element;
        }
    }
}
