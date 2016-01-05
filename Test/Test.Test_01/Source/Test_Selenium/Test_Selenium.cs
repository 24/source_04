using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using pb;
using pb.Compiler;
using pb.IO;
using System.Threading;

namespace Test.Test_Selenium
{
    public static class Test_Selenium
    {
        public static string GetChromeDriverDirectory()
        {
            return @"$Root$\..\library\Selenium\chrome.driver".zGetRunSourceProjectVariableValue().zRootPath(RunSource.CurrentRunSource.ProjectDirectory);
        }

        public static void Test_Selenium_01()
        {
            Trace.WriteLine("Test_Selenium_01");
        }

        public static void Test_Selenium_Firefox_01()
        {
            Trace.WriteLine("Test_Selenium_Firefox_01");

            // from http://www.seleniumhq.org/docs/03_webdriver.jsp

            // Create a new instance of the Firefox driver.

            // Notice that the remainder of the code relies on the interface, 
            // not the implementation.

            // Further note that other drivers (InternetExplorerDriver,
            // ChromeDriver, etc.) will require further configuration 
            // before this example will work. See the wiki pages for the
            // individual drivers at http://code.google.com/p/selenium/wiki
            // for further information.
            Trace.WriteLine("  IWebDriver driver = new FirefoxDriver();");
            IWebDriver driver = new FirefoxDriver();

            //Notice navigation is slightly different than the Java version
            //This is because 'get' is a keyword in C#
            Trace.WriteLine("  driver.Navigate().GoToUrl(\"http://www.google.com/\");");
            driver.Navigate().GoToUrl("http://www.google.com/");

            // Find the text input element by its name
            Trace.WriteLine("  IWebElement query = driver.FindElement(By.Name(\"q\"));");
            IWebElement query = driver.FindElement(By.Name("q"));

            // Enter something to search for
            Trace.WriteLine("  query.SendKeys(\"Cheese\");");
            query.SendKeys("Cheese");

            // Now submit the form. WebDriver will find the form for us from the element
            Trace.WriteLine("  query.Submit();");
            query.Submit();

            // Google's search is rendered dynamically with JavaScript.
            // Wait for the page to load, timeout after 10 seconds
            Trace.WriteLine("  WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));");
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            Trace.WriteLine("  wait.Until((d) => { return d.Title.ToLower().StartsWith(\"cheese\"); });");
            wait.Until((d) => { return d.Title.ToLower().StartsWith("cheese"); });

            // Should see: "Cheese - Google Search"
            //Trace.WriteLine("Page title is : " + driver.Title);
            Trace.WriteLine("  driver.Title : \"{0}\"", driver.Title);

            //Close the browser
            Trace.WriteLine("  driver.Quit();");
            driver.Quit();
        }

        public static void Test_Selenium_Chrome_01()
        {
            Trace.WriteLine("Test_Selenium_Chrome_01");

            // from http://www.seleniumhq.org/docs/03_webdriver.jsp

            // Create a new instance of the Firefox driver.

            // Notice that the remainder of the code relies on the interface, 
            // not the implementation.

            // Further note that other drivers (InternetExplorerDriver,
            // ChromeDriver, etc.) will require further configuration 
            // before this example will work. See the wiki pages for the
            // individual drivers at http://code.google.com/p/selenium/wiki
            // for further information.
            //string chromeDriverDirectory = @"$Root$\..\library\Selenium\chrome.driver".zGetRunSourceProjectVariableValue().zRootPath(RunSource.CurrentRunSource.ProjectDirectory);
            string chromeDriverDirectory = GetChromeDriverDirectory();
            Trace.WriteLine("  IWebDriver driver = new ChromeDriver(\"{0}\");", chromeDriverDirectory);
            //ChromeDriverService service = ChromeDriverService.CreateDefaultService(chromeDriverDirectory);
            //service.HideCommandPromptWindow = true;
            //service.LogPath
            IWebDriver driver = new ChromeDriver(chromeDriverDirectory);

            //Notice navigation is slightly different than the Java version
            //This is because 'get' is a keyword in C#
            Trace.WriteLine("  driver.Navigate().GoToUrl(\"http://www.google.com/\");");
            driver.Navigate().GoToUrl("http://www.google.com/");

            // Find the text input element by its name
            Trace.WriteLine("  IWebElement query = driver.FindElement(By.Name(\"q\"));");
            IWebElement query = driver.FindElement(By.Name("q"));

            // Enter something to search for
            Trace.WriteLine("  query.SendKeys(\"Cheese\");");
            query.SendKeys("Cheese");

            // Now submit the form. WebDriver will find the form for us from the element
            Trace.WriteLine("  query.Submit();");
            query.Submit();

            // Google's search is rendered dynamically with JavaScript.
            // Wait for the page to load, timeout after 10 seconds
            Trace.WriteLine("  WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));");
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            Trace.WriteLine("  wait.Until((d) => { return d.Title.ToLower().StartsWith(\"cheese\"); });");
            wait.Until((d) => { return d.Title.ToLower().StartsWith("cheese"); });

            // Should see: "Cheese - Google Search"
            //Trace.WriteLine("Page title is : " + driver.Title);
            Trace.WriteLine("  driver.Title : \"{0}\"", driver.Title);

            //Close the browser
            Trace.WriteLine("  driver.Quit();");
            driver.Quit();
        }

        public static void Test_Selenium_Chrome_02()
        {
            Trace.WriteLine("Test_Selenium_Chrome_02");

            // from http://www.seleniumhq.org/docs/03_webdriver.jsp

            // Create a new instance of the Firefox driver.

            // Notice that the remainder of the code relies on the interface, 
            // not the implementation.

            // Further note that other drivers (InternetExplorerDriver,
            // ChromeDriver, etc.) will require further configuration 
            // before this example will work. See the wiki pages for the
            // individual drivers at http://code.google.com/p/selenium/wiki
            // for further information.
            //string chromeDriverDirectory = @"$Root$\..\library\Selenium\chrome.driver".zGetRunSourceProjectVariableValue().zRootPath(RunSource.CurrentRunSource.ProjectDirectory);
            string chromeDriverDirectory = GetChromeDriverDirectory();
            Trace.WriteLine("  ChromeDriverService service = ChromeDriverService.CreateDefaultService(\"{0}\");", chromeDriverDirectory);
            ChromeDriverService service = ChromeDriverService.CreateDefaultService(chromeDriverDirectory);
            service.HideCommandPromptWindow = true;
            service.SuppressInitialDiagnosticInformation = true;
            Trace.WriteLine("  service.LogPath : \"{0}\"", service.LogPath);
            Trace.WriteLine("  service.Start();");
            // execute "chromedriver.exe"
            service.Start();
            Trace.WriteLine("  service.ProcessId : {0}", service.ProcessId);
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.LeaveBrowserRunning = true;
            Trace.WriteLine("  IWebDriver driver = new ChromeDriver(service, chromeOptions);");
            IWebDriver driver = new ChromeDriver(service, chromeOptions);
            Trace.WriteLine("  driver.CurrentWindowHandle : \"{0}\"", driver.CurrentWindowHandle);
            Trace.WriteLine("  driver.WindowHandles.Count : {0}", driver.WindowHandles.Count);
            foreach (string windowHandle in driver.WindowHandles)
                Trace.WriteLine("  windowHandle : \"{0}\"", windowHandle);

            //Notice navigation is slightly different than the Java version
            //This is because 'get' is a keyword in C#
            Trace.WriteLine("  driver.Navigate().GoToUrl(\"http://www.google.com/\");");
            driver.Navigate().GoToUrl("http://www.google.com/");

            // Find the text input element by its name
            Trace.WriteLine("  IWebElement query = driver.FindElement(By.Name(\"q\"));");
            IWebElement query = driver.FindElement(By.Name("q"));

            // Enter something to search for
            Trace.WriteLine("  query.SendKeys(\"Cheese\");");
            query.SendKeys("Cheese");

            // Now submit the form. WebDriver will find the form for us from the element
            Trace.WriteLine("  query.Submit();");
            query.Submit();

            // Google's search is rendered dynamically with JavaScript.
            // Wait for the page to load, timeout after 10 seconds
            Trace.WriteLine("  WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));");
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            Trace.WriteLine("  wait.Until((d) => { return d.Title.ToLower().StartsWith(\"cheese\"); });");
            wait.Until((d) => { return d.Title.ToLower().StartsWith("cheese"); });

            // Should see: "Cheese - Google Search"
            //Trace.WriteLine("Page title is : " + driver.Title);
            Trace.WriteLine("  driver.Title : \"{0}\"", driver.Title);

            Trace.WriteLine("  Thread.Sleep(30000);");
            Thread.Sleep(30000);

            //Close the browser
            Trace.WriteLine("  driver.Quit();");
            driver.Quit();
            service.Dispose();
        }
    }
}
