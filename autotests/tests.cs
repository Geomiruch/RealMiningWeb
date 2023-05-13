using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading.Tasks;

[TestClass]
public class FilesControllerTests
{
    private IWebDriver driver;
    private string baseURL = "http://localhost:5000"; 

    [TestInitialize]
    public void TestInitialize()
    {
        driver = new ChromeDriver();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        driver.Quit();
    }

    [TestMethod]
    public async Task Index_SortByMod_ShouldReturnFilesSortedByModeVersion()
    {
        // Arrange
        driver.Navigate().GoToUrl(baseURL + "/Files");

        // Act
        var sortByModLink = driver.FindElement(By.LinkText("Sort by Mode Version"));
        sortByModLink.Click();

        await Task.Delay(1000); // Wait for the page to load

        // Assert
        var tableRows = driver.FindElements(By.XPath("//table[@id='filesTable']/tbody/tr"));
        var firstRowColumns = tableRows[0].FindElements(By.TagName("td"));

        Assert.AreEqual("Mod1.0", firstRowColumns[1].Text);
    }
}
[Fact]
public async Task ForgotPassword_ValidInput_ReturnsRedirectToActionResult()
{
    // Arrange
    var controller = new AccountController(db, Environment);
    var model = new RegisterModel { Login = "johndoe", Password = "new_password" };

    // Act
    var result = await controller.ForgotPassword(model) as RedirectToActionResult;

    // Assert
    Assert.NotNull(result);
    Assert.Equal("Login", result.ActionName);
    Assert.Equal("Account", result.ControllerName);
}

[Fact]
public async Task Register_InvalidInput_ReturnsViewResult()
{
    // Arrange
    var controller = new AccountController(db, Environment);
    var model = new RegisterModel { Login = "johndoe", Password = "password" };

    // Act
    var result = await controller.Register(model, null) as ViewResult;

    // Assert
    Assert.NotNull(result);
    Assert.True(result.ViewData.ModelState.Count == 3);
    Assert.True(result.ViewData.ModelState.ContainsKey("FirstName"));
    Assert.True(result.ViewData.ModelState.ContainsKey("LastName"));
    Assert.True(result.ViewData.ModelState.ContainsKey("RoleID"));
}
[Fact]
public async Task Register_ValidInput_ReturnsRedirectToActionResult()
{
    // Arrange
    var controller = new AccountController(db, Environment);
    var model = new RegisterModel { FirstName = "John", LastName = "Doe", Login = "johndoe", Password = "password", RoleID = 1 };

    // Act
    var result = await controller.Register(model, null) as RedirectToActionResult;

    // Assert
    Assert.NotNull(result);
    Assert.Equal("Login", result.ActionName);
    Assert.Equal("Account", result.ControllerName);
}
[Fact]
public async Task Login_InvalidInput_ReturnsViewResult()
{
    // Arrange
    var controller = new AccountController(db, Environment);
    var model = new LoginModel { Login = "invalid_login", Password = "invalid_password" };

    // Act
    var result = await controller.Login(model) as ViewResult;

    // Assert
    Assert.NotNull(result);
    Assert.True(result.ViewData.ModelState.Count == 1);
    Assert.True(result.ViewData.ModelState.ContainsKey(""));
    Assert.Equal("Некорректные логин и(или) пароль", result.ViewData.ModelState[""].Errors[0].ErrorMessage);
}
[Fact]
public void Login_ValidInput_ReturnsViewResult()
{
    // Arrange
    var controller = new AccountController(db, Environment);

    // Act
    var result = controller.Login() as ViewResult;

    // Assert
    Assert.NotNull(result);
}
[Test]
public void Login_ReturnsLoginView()
{
    // Arrange
    var controller = new AccountController();

    // Act
    var result = controller.Login() as ViewResult;

    // Assert
    Assert.That(result.ViewName, Is.EqualTo("Login"));
}
[Test]
public async Task Login_ValidUser_RedirectsToHomeIndex()
{
    // Arrange
    var context = new Mock<HttpContext>();
    var controller = new AccountController(context.Object);
    var user = new User
    {
        Login = "test",
        Password = "testpassword"
    };
    var loginModel = new LoginModel
    {
        Login = "test",
        Password = "testpassword"
    };
    var claims = new List<Claim>
    {
        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
        new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
    };
    var identity = new ClaimsIdentity(claims, "Test");
    var principal = new ClaimsPrincipal(identity);

    // Act
    var result = await controller.Login(loginModel) as RedirectToActionResult;

    // Assert
    Assert.That(result.ActionName, Is.EqualTo("Index"));
    Assert.That(result.ControllerName, Is.EqualTo("Home"));
}
[Test]
public async Task Login_InvalidUser_ReturnsLoginViewWithErrorMessage()
{
    // Arrange
    var context = new Mock<HttpContext>();
    var controller = new AccountController(context.Object);
    var loginModel = new LoginModel
    {
        Login = "invalid",
        Password = "password"
    };

    // Act
    var result = await controller.Login(loginModel) as ViewResult;

    // Assert
    Assert.That(result.ViewName, Is.EqualTo("Login"));
    Assert.That(result.ViewData.ModelState.IsValid, Is.False);
    Assert.That(result.ViewData.ModelState[""].Errors.First().ErrorMessage, Is.EqualTo("Некорректные логин и(или) пароль"));
}
[Test]
public void Register_ReturnsRegisterView()
{
    // Arrange
    var controller = new AccountController();

    // Act
    var result = controller.Register() as ViewResult;

    // Assert
    Assert.That(result.ViewName, Is.EqualTo("Register"));
}

