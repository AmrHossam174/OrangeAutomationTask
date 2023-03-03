using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace Orange_TechnicalTest
{
    class TestClass
    {
        IWebDriver driver = new ChromeDriver();


        [SetUp]
        public void Intialize()
        {
            //Maximize the window
            driver.Manage().Window.Maximize();

            //Navigating To Amazon
            driver.Navigate().GoToUrl("https://www.amazon.eg/");

        }

        [Test]
        public void ExecuteTest()
        {
            //TurnToEnglish();

            //*LOCATING ELEMENTS*
            GetAmazonTitle();
            AssertAmazonEg();

            //SearchBar
            MobileSearch();
            AssertMobileSearch();


            //Filtering  *Note: "HTC" was not there so i replaced it by "Redmi"... To get HTC -> just replace "Redmi" with "HTC"  
            FilterMobile("Redmi");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            AssertChecboxChecked();


            //Prices - It Take Two Values (Low Range,High Range)
            EnterPriceRange("3500", "5000");
            
            ClickOnGoBtn();

            //The Prices is seperated by comma as shown below
            AssertPriceRanges("3,500", "5,000");

            SelectLastMobile();
            
            AddToCart();
            AssertItemAddedToCart();

            GoToCart();


            DeleteFromCart();
            AssertItemIsDeleted();


        }


       

        [TearDown]
        public void closing()
        {
            driver.Close();
        }

       


        /*-------------------------------------------Locators------------------------------------------------*/
        public String GetAmazonTitle()
        {
            IWebElement Amazon = driver.FindElement(By.XPath("//*[@aria-label='Amazon.eg']"));

            return Amazon.Text;
        }

        //For Testing In English Language 
        public void TurnToEnglish()
        {
            IWebElement English = driver.FindElement(By.CssSelector("#icp-nav-flyout"));
            English.Click();

            IWebElement EngChoice = driver.FindElement(By.CssSelector("#icp-language-settings > div:nth-child(6) > div > label > i"));
            EngChoice.Click();
       
            IWebElement SaveChoice = driver.FindElement(By.XPath("//*[@aria-labelledby='icp-save-button-announce']"));
            SaveChoice.Click();
        }

        public void MobileSearch()
        {
            //SearchBar
            IWebElement SearchBar = driver.FindElement(By.XPath("//*[@id='twotabsearchtextbox']"));
            SearchBar.SendKeys("Mobile");
            SearchBar.SendKeys(Keys.Enter);
        }

        //Filtering  *Note: "HTC" was not there so i replaced it by "Redmi"... To get HTC -> just replace "Redmi" with "HTC"  
        public void FilterMobile(String MobileName)
        {
            IWebElement FilterMobile = driver.FindElement(By.XPath("//*[contains(@id," + "'"+MobileName+"'" + ")]//a"));
            
            try //if the option was avaliable without need of exapnding menu
            {

                FilterMobile.Click();
            }

            catch // if the menu needing exapansion
            {
                IWebElement MoreOptions = driver.FindElement(By.CssSelector("#brandsRefinements > ul > li:nth-child(8) > span > div > a "));
                MoreOptions.Click();

                FilterMobile.Click();
            }

        }

        public void EnterPriceRange(String LowRange , String HighRange)
        {
            IWebElement LowPrice = driver.FindElement(By.CssSelector("#low-price"));
            LowPrice.SendKeys(LowRange);
            IWebElement HighPrice = driver.FindElement(By.CssSelector("#high-price"));
            HighPrice.SendKeys(HighRange);
        }

        //Go Button
        public void ClickOnGoBtn()
        {
            IWebElement GoBtn = driver.FindElement(By.CssSelector("form>span>span>input.a-button-input"));
            GoBtn.Click();
        }

        //Selecting Last Mobile On The Page
      

        /*There is two ads that was out of the result but thier attribute @data-asin was empty , thus @data-asin!=' ' return only the
        required mobiles*/
        public void SelectLastMobile()
        {
          
            IWebElement LastMobile = driver.FindElement(By.XPath("//div[contains(@class,'s-search-results')]/div[@data-asin!=''][last()]"));
            LastMobile.Click();
        }

        public void AddToCart()
        {
            //Add To Cart
            IWebElement AddToCartBtn = driver.FindElement(By.CssSelector("input#add-to-cart-button"));
            AddToCartBtn.Click();

        }

        public void GoToCart()
        {

            //Go to Cart
            IWebElement GoToCartBtn = driver.FindElement(By.CssSelector("#nav-cart"));
            GoToCartBtn.Click();
        }

        public void DeleteFromCart()
        {
            //Deleting Item
            IWebElement DeleteBtn = driver.FindElement(By.XPath("//span[@data-feature-id='delete']/span/input"));
            DeleteBtn.Click();
        }
        /* ------------------------------------------------------------------------------------------------------------------*/


        /*------------------------------------ Assertions---------------------------------------------------------------------*/

        public void AssertAmazonEg()
        {
            Assert.AreEqual(GetAmazonTitle(), ".eg", "The Amazon egypt Website Failed To Open As Expected");
        }

        public void AssertMobileSearch()
        {
            IWebElement SearchValue = driver.FindElement(By.CssSelector("#twotabsearchtextbox"));
            String valueOfSearch = SearchValue.GetAttribute("value");
            Assert.AreEqual(valueOfSearch, "Mobile", "The Value Mobile was not entered as expected");
        }

        public void AssertChecboxChecked()
        {
            IWebElement FilterMobileCheckbox = driver.FindElement(By.XPath("//*[contains(@id,'Redmi')]//a//input"));
            Assert.True(FilterMobileCheckbox.Selected,"The Expected Mobile was NOT selected");
        }

        public void AssertPriceRanges(String LowValue,String HighValue)
        {
            IWebElement LowPrice = driver.FindElement(By.CssSelector("#low-price"));
            IWebElement HighPrice = driver.FindElement(By.CssSelector("#high-price"));


            Assert.AreEqual(LowPrice.GetAttribute("value"), LowValue, "The Low Price Value was entered INCORRECTLY");
            Assert.AreEqual(HighPrice.GetAttribute("value"), HighValue, "The High Price Value was entered INCORRECTLY");

        }

        public void AssertItemAddedToCart()
        {
            IWebElement CartCount = driver.FindElement(By.CssSelector("#nav-cart-count"));
            Assert.AreEqual(CartCount.Text, "1", "The Item was NOT added to cart as Expected");
        }

        /*when the item is deleted the style in below element is empty , if not then the style=Display:none;
        therefore asserting if the style is empty will ensure that the item is deleted*/
        public void AssertItemIsDeleted()
        {
            IWebElement DeletedItem = driver.FindElement(By.CssSelector(".sc-list-item-removed-msg"));
           String Style= DeletedItem.GetAttribute("style");
            Assert.IsEmpty(Style);
        }

    }
}
