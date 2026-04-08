using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using OpenQA.Selenium.Interactions;

namespace test_1
{
    [TestFixture]
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

            var login = wait.Until(
                ExpectedConditions.ElementIsVisible(By.Id("Username"))
            );
            login.Clear();
            login.SendKeys("denis1den12nis@gmail.com");

            var password = wait.Until(
                ExpectedConditions.ElementIsVisible(By.Id("Password"))
            );
            password.Clear();
            password.SendKeys("Vangog1984!");

            var enter = wait.Until(
                ExpectedConditions.ElementToBeClickable(By.CssSelector("[class='form-button']"))
            );
            enter.Click();

            wait.Until(
                ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Title']"))
            );
        }

        [Test]
public void UserCannotSendCommentLongerThan5001Characters()
{
    // Авторизация пользователя в системе
    Authorize();

    // Нажимаем кнопку добавления комментария
    var addCommentButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[data-tid='AddComment']")));
    addCommentButton.Click();

    // Создаем строку длиной 5001 символ
    var commentsText = new string('a', 5001);

    // Вводим текст комментария
    var commentInput = driver.FindElement(By.CssSelector("[placeholder='Комментировать...']"));
    commentInput.SendKeys(commentsText);

    // Отправляем комментарий с помощью Tab + Enter
    new Actions(driver).SendKeys(Keys.Tab).SendKeys(Keys.Enter).Perform();

    // Находим список комментариев
    var commentsList = driver.FindElement(By.CssSelector("[data-tid='CommentsList']"));

    // Получаем все текстовые комментарии
    var comments = commentsList.FindElements(By.CssSelector("[data-tid='TextComment']"));

    // Берем последний комментарий в списке
    var myComment = comments.Last();

    // Проверяем, что комментарий с текстом длиной 5001 символ не был отправлен
    Assert.That(myComment.Text, !Does.Contain(commentsText),
        $"Вместо введенного текста: '{commentsText}'. Отображается: '{myComment.Text}'");
}

[Test]
public void UserCanUploadPngPhotoToProfile()
{
    // Авторизация пользователя в системе
    Authorize();

    // Переходим на страницу редактирования профиля
    driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/profile/settings/edit");

    // Ожидаем, пока URL страницы будет содержать путь редактирования профиля
    wait.Until(ExpectedConditions.UrlContains("/profile/settings/edit"));

    // Указываем путь к PNG-файлу для загрузки
    var filePath = @"C:\Users\Денис\OneDrive\Desktop\111.png";

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

    // Завершаем тест сообщением об успешной отправке файла
    Assert.Pass("Файл был успешно выбран и отправлен на загрузку");

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
        public void Authorization_test() 
        {// тест авторизации
        Authorize();
        Assert.That(driver.Title, Does.Contain("Новости")); // проверка, что открыта нужная страница
        }

    [Test]
    public void ProfileOpen() // Тестирование, открытия меню профиля
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
    public void SearchTest() // тест поисковой строки
    {

    Authorize();
    wait.Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/news")); // ожидание перехо

    var search = driver.FindElement(By.CssSelector("[data-tid='SearchBar']")); // поиск строки поиска
    search.Click(); // клик по строке поиска

    var searchInput = driver.FindElement(By.CssSelector("[placeholder='Поиск сотрудника, подразделения, сообщества, мероприятия']"));
    searchInput.SendKeys("агапова алиса алексеевна "); // ввод запроса
    }
}
}
