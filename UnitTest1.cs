using Newtonsoft.Json.Linq;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Data.Common;
using System.Globalization;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using static System.Collections.Specialized.BitVector32;
using SeleniumExtras.WaitHelpers;
using static OpenQA.Selenium.BiDi.Modules.Input.Wheel;

namespace LocatorsPractice
{
    public class Tests
    {
        public IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.epam.com");
            driver.Manage().Window.Maximize();

            // Accept cookies (Ensure this runs before interacting with elements)
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                IWebElement AcceptBtn = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("onetrust-accept-btn-handler")));
                AcceptBtn.Click();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("onetrust-banner-sdk")));
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Cookies banner not found, proceeding...");
            }
        }

        [Test]
        [TestCase("SAP", "Colombia")]
        [TestCase("Python", "All Locations")]
        [TestCase("C#", "All Locations")]
        public void Test1(String programmingLanguage, string location)
        {
            try
            {
                // Find a link “Carriers” and click on it
                IWebElement CareersBttn = driver.FindElement(By.XPath("//li[@class='top-navigation__item epam'][5]"));
                CareersBttn.Click();

                // Write the name of any programming language in the field “Keywords” (should be taken from test parameter)
                IWebElement KeywordsField = driver.FindElement(By.Id("new_form_job_search-keyword"));
                KeywordsField.SendKeys(programmingLanguage);

                //Select “All Locations” in the “Location” field(should be taken from the test parameter)
                IWebElement LocationField = driver.FindElement(By.XPath("//span[@class='select2-selection__rendered']"));
                LocationField.Click();
                driver.FindElement(By.XPath($"//li[contains(text(),'{location}')]")).Click(); ////li[@title='{location}']

                //Select the option “Remote”
                IWebElement RemoteCheck = driver.FindElement(By.XPath("//label[@for='id-93414a92-598f-316d-b965-9eb0dfefa42d-remote']"));
                RemoteCheck.Click();

                //Click on the button “Find”
                IWebElement FindBttn = driver.FindElement(By.XPath("//button[@type='submit']"));
                FindBttn.Click();

                //Explicit wait implemented , to interact with the loader animation
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                Actions actions = new Actions(driver);
                
                while (true) 
                {
                    // Wait for loading animation to disappear
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(".preloader")));
                    // Scroll to the last item using Actions class
                    IWebElement LastItem = driver.FindElement(By.XPath("//ul[@class='search-result__list']/li[last()]//div//div/a"));
                    actions.ScrollToElement(LastItem).Perform();
                    // Check if the loading animation appears again
                    var isLoadingVisible = driver.FindElements(By.CssSelector(".preloader")).Any(e => e.Displayed);
                    if (!isLoadingVisible)
                    {
                        LastItem = driver.FindElement(By.XPath("//ul[@class='search-result__list']/li[last()]//div//div/a"));
                        actions.ScrollToElement(LastItem).Perform();
                        LastItem.Click();
                        break;
                    }
                }

                //Validate that the programming language mentioned in the step above is on a page
                var Requirements = driver.FindElement(By.XPath("//div[@class='vacancy-details-23__content-holder']/ul[2]"));

                //Find all <li> items inside the list
                var listItems = Requirements.FindElements(By.TagName("li"));

                //Check if any <li> contains "C#"
                bool containsLanguage = listItems.Any(item => item.Text.Contains(programmingLanguage, StringComparison.OrdinalIgnoreCase));

                if (containsLanguage)
                {
                    Console.WriteLine($"Test Passed: Found '{programmingLanguage}' in Requirements description.");
                    Assert.Pass();
                }
                else
                {
                    Console.WriteLine($"Test Failed: '{programmingLanguage}' not found in Requirements description.");
                    Assert.Fail();
                }
            }
            finally { 
            
                driver.Quit();
            }
        }

        [Test]
        [TestCase("CLOUD")]
        [TestCase("BLOCKCHAIN")]
        [TestCase("Automation")]
        public void Test2(string searchString) 
        {
            try 
            {
                //Find the magnifier Icon and click on it
                IWebElement magnifierIcon = driver.FindElement(By.XPath("//button[@class='header-search__button header__icon']"));
                magnifierIcon.Click();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("new_form_search")));
                
                // Find a search string and put there “BLOCKCHAIN”/”Cloud”/”Automation” (use as a parameter for a test)
                IWebElement searchBox = driver.FindElement(By.Id("new_form_search"));
                searchBox.SendKeys(searchString);

                //Click 'Find' button
                IWebElement findBttn = driver.FindElement(By.CssSelector(".custom-button"));
                findBttn.Click();
                
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(".preloader")));
                
                //Scroll down to the Page Footer
                IWebElement pageFooter = driver.FindElement(By.XPath("//div[@class='footer-inner']"));
                new Actions(driver).ScrollToElement(pageFooter).Perform();
                           
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(text(),'View More')]")));

                // First Scroll to view more bttn
                IWebElement viewMoreBttn = driver.FindElement(By.XPath("//span[contains(text(),'View More')]"));
                new Actions(driver).ScrollToElement(viewMoreBttn).Perform();

                // This loop executes until all the found items on the list are displayed
                while (viewMoreBttn.Displayed) 
                {
                    viewMoreBttn.Click();
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(".preloader")));
                    if (viewMoreBttn.Displayed) 
                    {
                        new Actions(driver).ScrollToElement(viewMoreBttn).Perform();
                    }
                    else 
                    {
                        break;
                    }
                }

                //Find all links displayed text
                var linksText = driver.FindElements(By.XPath("//a[@class='search-results__title-link']"));

                // Extract text from each element and store it in a list
                List<string> texts = linksText.Select(e => e.Text).ToList();

                // Count amount of items that inclide the searchString in the text link
                
                int countContaining = texts.Count(text => text.Contains(searchString, StringComparison.OrdinalIgnoreCase));
                int countNotContaining = texts.Count(text => !text.Contains(searchString, StringComparison.OrdinalIgnoreCase));

                //Validate that all the items contain the searchString
                bool allContainSearchString = texts.All(text => text.Contains(searchString, StringComparison.OrdinalIgnoreCase));

                if (allContainSearchString) 
                {
                    Assert.Pass();
                    Console.WriteLine($"All the link´s text contain the search value: {searchString}");
                }
                else
                {
                    Console.WriteLine($"{countContaining} link´s text contain the {searchString} value");
                    Console.WriteLine($"{countNotContaining} link´s text do not contain the {searchString} value");
                    Assert.Fail();
                }
            }
            finally 
            {
                driver.Quit();
            }
        }

    }
}