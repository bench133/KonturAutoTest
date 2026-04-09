using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using OpenQA.Selenium.Interactions;

namespace test_1
{
    public class LoginTests
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void SetUp()
        {
            var options = new EdgeOptions();
            options.AddArgument("start-maximized");
            options.AcceptInsecureCertificates = true;

            driver = new EdgeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(100));
        }

        [TearDown]
        public void TearDown()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
            }
        }

        private void Authorize()
        {
            driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/");
        
            var userLogin = Environment.GetEnvironmentVariable("TEST_LOGIN");
            var userPassword = Environment.GetEnvironmentVariable("TEST_PASSWORD");
        
            Assert.That(userLogin, Is.Not.Null.And.Not.Empty, "Не задана переменная окружения TEST_LOGIN");
            Assert.That(userPassword, Is.Not.Null.And.Not.Empty, "Не задана переменная окружения TEST_PASSWORD");
        
            var login = wait.Until(
                ExpectedConditions.ElementIsVisible(By.Id("Username"))
            );
            login.Clear();
            login.SendKeys(userLogin);
        
            var password = wait.Until(
                ExpectedConditions.ElementIsVisible(By.Id("Password"))
            );
            password.Clear();
            password.SendKeys(userPassword);
        
            var enter = wait.Until(
                ExpectedConditions.ElementToBeClickable(By.CssSelector("button[name='button']"))
            );
            enter.Click();
        
            wait.Until(
                ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Title']"))
            );
        }
        
        [Test]
        public void UserCannotSendInvalidCommentTest()
        {
            // Авторизация пользователя в системе
            Authorize();
        
            // Получаем кнопку с количеством комментариев
            var commentsToggleBefore = wait.Until(
                ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='CommentsToggle']"))
            );
        
            // Сохраняем текст кнопки до отправки
            var commentsCountBeforeText = commentsToggleBefore.Text;
        
            // Нажимаем кнопку добавления комментария
            var addCommentButton = wait.Until(
                ExpectedConditions.ElementToBeClickable(By.CssSelector("[data-tid='AddComment']"))
            );
            addCommentButton.Click();
        
            // Создаем строку длиной 5001 символ
            var commentsText = new string('a', 5001);
        
            // Вводим слишком длинный комментарий
            var commentInput = wait.Until(
                ExpectedConditions.ElementIsVisible(By.CssSelector("[placeholder='Комментировать...']"))
            );
            commentInput.SendKeys(commentsText);
        
            // Пытаемся отправить комментарий
            new Actions(driver).SendKeys(Keys.Tab).SendKeys(Keys.Enter).Perform();
        
            // Снова получаем кнопку с количеством комментариев
            var commentsToggleAfter = wait.Until(
                ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='CommentsToggle']"))
            );
        
            // Сохраняем текст кнопки после отправки
            var commentsCountAfterText = commentsToggleAfter.Text;
        
            // Проверяем, что количество комментариев не изменилось
            Assert.That(
                commentsCountAfterText,
                Is.EqualTo(commentsCountBeforeText),
                $"Комментарий длиной 5001 символ не должен быть отправлен. " +
                $"До отправки: '{commentsCountBeforeText}', после отправки: '{commentsCountAfterText}'."
            );
        }
    
    [Test]
    public void UserUploadPhotoProfileTest()
    {
        // Авторизация пользователя в системе
        Authorize();
    
        // Переходим на страницу редактирования профиля
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/profile/settings/edit");
    
        // Ожидаем, пока URL страницы будет содержать путь редактирования профиля
        wait.Until(ExpectedConditions.UrlContains("/profile/settings/edit"));
    
        // Указываем путь к PNG-файлу для загрузки
        var filePath = @"C:\Users\Денис\OneDrive\Desktop\111.png"; //я знаю, что файл возьмется с моего компьютера только
    
        // Проверяем, что файл существует по указанному пути
        Assert.That(
            System.IO.File.Exists(filePath),
            Is.True,
            $"Файл не найден по пути: {filePath}"
        );
    
        // Нажимаем кнопку выбора фотографии
        var uploadButton = wait.Until(
            ExpectedConditions.ElementToBeClickable(By.CssSelector("[data-tid='UploadFiles']"))
        );
        uploadButton.Click();
    
        // Находим input для загрузки файла
        var fileInput = wait.Until(
            ExpectedConditions.ElementExists(By.CssSelector("input[type='file']"))
        );
    
        // Делаем input видимым через JavaScript
        ((IJavaScriptExecutor)driver).ExecuteScript(
            "arguments[0].style.display='block'; arguments[0].style.visibility='visible';",
            fileInput
        );
    
        // Загружаем файл по указанному пути
        fileInput.SendKeys(filePath);
    
        // Нажимаем кнопку сохранения
        var saveButton = wait.Until(
            ExpectedConditions.ElementToBeClickable(
                By.XPath("//section[@data-tid='PageHeader']//button[contains(., 'Сохранить')]")
            )
        );
        saveButton.Click();
        // Получаем имя загруженного файла через JavaScript
        var uploadedFileName = ((IJavaScriptExecutor)driver).ExecuteScript(
            "return arguments[0].files[0].name;",
            fileInput
        )?.ToString();
    
        // Проверяем, что загружен именно файл 111.png
        Assert.That(
            uploadedFileName,
            Is.EqualTo("111.png"),
            $"Ожидался файл '111.png', но получено '{uploadedFileName}'"
        );
    }
    [Test]
    public void AuthorizationTest()
    {
        // Авторизация пользователя в системе
        Authorize();
        // Находим заголовок страницы после успешной авторизации
        var titleElement = wait.Until(
            ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Title']"))
        );
        // Проверяем, что после входа открыт раздел "Новости"
        Assert.That(titleElement.Text, Does.Contain("Новости"), $"После успешной авторизации ожидался раздел 'Новости', но отображается: '{titleElement.Text}'");
    }
    
        [Test]
        public void  UserCanOpenOwnProfileTest() // Тестирование, открытия меню профиля
        {
            Authorize();
            // Открытие выпадающего меню профилья
            var profileMenuButton =
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[data-tid='ProfileMenu'] button")));
            profileMenuButton.Click();
    
            // Переход на страницу "Профиль"
            var settingsItem = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[data-tid='Profile']")));
            settingsItem.Click();
    
            // Проверка, что открыт профиль нужного пользователя
            var fioElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='EmployeeName']")));
            Assert.That(fioElement.Text, Is.EqualTo("Хамидулин Денис Альбертович"), "Открыт профиль другого пользователя");
        }
    
    [Test]
    public void UserCanSearchEmployeeTest()
    {
        // Авторизуемся в системе
        Authorize();
    
        // Ожидаем, что после авторизации откроется страница новостей
        wait.Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/news"));
    
        // Находим строку поиска и дожидаемся, пока она станет кликабельной
        var search = wait.Until(
            ExpectedConditions.ElementToBeClickable(By.CssSelector("[data-tid='SearchBar']"))
        );
    
        // Кликаем по строке поиска
        search.Click();
    
        // Находим поле ввода поискового запроса и дожидаемся, пока оно станет видимым
        var searchInput = wait.Until(
            ExpectedConditions.ElementIsVisible(
                By.CssSelector("[placeholder='Поиск сотрудника, подразделения, сообщества, мероприятия']")
            )
        );
    
        // Задаем поисковый запрос
        var query = "агапова алиса алексеевна";
    
        // Вводим поисковый запрос в поле поиска
        searchInput.SendKeys(query);
    
        // Ожидаем появления результата поиска с нужным именем сотрудника
        var searchResult = wait.Until(
            ExpectedConditions.ElementIsVisible(
                By.XPath("//*[contains(text(),'Агапова Алиса Алексеевна')]")
            )
        );
    
        // Проверяем, что в результатах поиска отображается нужный сотрудник
        Assert.That(
            searchResult.Text,
            Does.Contain("Агапова Алиса Алексеевна"),
            $"После ввода '{query}' сотрудник не найден в результатах поиска."
        );
    }
    }
    }
