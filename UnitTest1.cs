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
                IWebElement AcceptBtn = driver.FindElement(By.Id("onetrust-accept-btn-handler"));
                AcceptBtn.Click();
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Cookies banner not found, proceeding...");
            }
        }

        [Test]
        public void Test1()
        {
            try
            {
                // Retrieve test parameters for Keywords and Location
                string programmingLanguage = TestContext.Parameters.Get("keyword", "c#"); // Default to "C#" if not provided
                string location = TestContext.Parameters.Get("location", "All Locations"); // Default to "All Locations"
                              
                // Find a link “Carriers” and click on it
                IWebElement CareersBttn = driver.FindElement(By.XPath("//li[@class='top-navigation__item epam'][5]"));
                CareersBttn.Click();

                // Write the name of any programming language in the field “Keywords” (should be taken from test parameter)
                IWebElement KeywordsField = driver.FindElement(By.Id("new_form_job_search-keyword"));
                KeywordsField.SendKeys(programmingLanguage);

                //Select “All Locations” in the “Location” field(should be taken from the test parameter)
                IWebElement LocationField = driver.FindElement(By.XPath("//span[@class='select2-selection__rendered']"));
                LocationField.Click();
                driver.FindElement(By.XPath($"//li[@title='{location}']")).Click();

                //Select the option “Remote”
                IWebElement RemoteCheck = driver.FindElement(By.XPath("//label[@for='id-93414a92-598f-316d-b965-9eb0dfefa42d-remote']"));
                RemoteCheck.Click();

                //Click on the button “Find”
                IWebElement FindBttn = driver.FindElement(By.XPath("//button[@type='submit']"));
                FindBttn.Click();

                Thread.Sleep(2000); // Wait for the initial results to load


                //Scroll down to one of the labels on the list
                IWebElement RelevanceLabel = driver.FindElement(By.XPath("//span[contains(text(),'Register your interest')]"));
                new Actions(driver).ScrollToElement(RelevanceLabel).Perform();

                // Wait till the page fully loads all results
                Thread.Sleep(5000);

                //Locate the latest item on the list and click on the 'View and Apply' button
                IWebElement LastItem = driver.FindElement(By.XPath("//ul[@class='search-result__list']/li[last()]/div/div/div/div/a"));
                LastItem.Click();

                //Validate that the programming language mentioned in the step above is on a page
                var Responsabilities = driver.FindElement(By.XPath("//div[@class='vacancy-details-23__content-holder']/ul[1]"));

                //Find all <li> items inside the list
                var listItems = Responsabilities.FindElements(By.TagName("li"));

                //Check if any <li> contains "C#"
                bool containsLanguage = listItems.Any(item => item.Text.Contains(programmingLanguage, StringComparison.OrdinalIgnoreCase));

                if (containsLanguage)
                {
                    Console.WriteLine($"Test Passed: Found '{programmingLanguage}' in responsabilities description.");
                    Assert.Pass();
                }
                else
                {
                    Console.WriteLine($"Test Failed: '{programmingLanguage}' not found in responsabilities description.");
                    Assert.Fail();
                }
            }
            finally { 
            
                driver.Quit();
            }
        }

        [Test]
        public void Test2() 
        {
            try 
            {
                //Test Parameters
                string searchString = TestContext.Parameters.Get("keyword", "Blockchain"); // BLOCKCHAIN”/”Cloud”/”Automation” Default to "BLOCKCHAIN" if not provided

                //Find the magnifier Icon and click on it
                IWebElement magnifierIcon = driver.FindElement(By.XPath("//button[@class='header-search__button header__icon']"));
                magnifierIcon.Click();

                Thread.Sleep(1000); // wait 1 second fot the searchbox to be visible

                // Find a search string and put there “BLOCKCHAIN”/”Cloud”/”Automation” (use as a parameter for a test)
                IWebElement searchBox = driver.FindElement(By.Id("new_form_search"));
                searchBox.SendKeys(searchString);

                //Click 'Find' button
                IWebElement findBttn = driver.FindElement(By.CssSelector(".custom-button"));
                findBttn.Click();

                Thread.Sleep(1000);

                //Scroll down to the Page Footer
                IWebElement pageFooter = driver.FindElement(By.XPath("//div[@class='footer-inner']"));
                new Actions(driver).ScrollToElement(pageFooter).Perform();
                
                // First Scroll to view more bttn
                IWebElement viewMoreBttn = driver.FindElement(By.XPath("//span[contains(text(),'View More')]"));
                new Actions(driver).ScrollToElement(viewMoreBttn).Perform();

                // This loop executes until all the found items on the list are displayed
                while (viewMoreBttn.Displayed) 
                {
                    viewMoreBttn.Click() ;
                    Thread.Sleep(1000); // Wait 1 second till new results load on the page.
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