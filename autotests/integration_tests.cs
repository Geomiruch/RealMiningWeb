using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Threading.Tasks;
using User_Story.Controllers;
using User_Story.Data;

namespace User_Story.IntegrationTests.Controllers
{
    [TestFixture]
    public class GuidesControllerTests
    {
        private GuidesController _controller;
        private Context _context;

        [SetUp]
        public void SetUp()
        {
            var services = new ServiceCollection();
            services.AddDbContext<Context>(options => options.UseInMemoryDatabase(databaseName: "TestDb"));
            var serviceProvider = services.BuildServiceProvider();
            _context = serviceProvider.GetRequiredService<Context>();
            _controller = new GuidesController(_context);
        }

        [Test]
        public async Task Index_ReturnsView()
        {
            // Arrange

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Index_ReturnsIndexView()
        {
            // Arrange

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.AreEqual("Index", result.ViewName);
        }

        [Test]
        public async Task Index_UsesContextToRetrieveData()
        {
            // Arrange
            _context.Guides.Add(new Guide { Id = 1, Name = "Guide 1" });
            _context.SaveChanges();

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result.Model);
        }

        [Test]
        public async Task Index_PassesRetrievedDataToView()
        {
            // Arrange
            _context.Guides.Add(new Guide { Id = 1, Name = "Guide 1" });
            _context.SaveChanges();

            // Act
            var result = await _controller.Index() as ViewResult;
            var model = result.Model as IEnumerable<Guide>;

            // Assert
            Assert.AreEqual(1, model.Count());
            Assert.AreEqual("Guide 1", model.First().Name);
        }

        [Test]
        public async Task Index_HandlesExceptionsGracefully()
        {
            // Arrange
            _context.Dispose(); // Simulate an error by disposing the context

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.AreEqual("Error", result.ViewName);
        }
    
    }

    [TestFixture]
    public class HomeControllerTests
    {
        private HomeController _controller;
        private Mock<ILogger<HomeController>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_loggerMock.Object);
        }

        [Test]
        public void Index_ReturnsViewResult()
        {
            // Arrange

            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Index_ReturnsView()
        {
            // Arrange

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.AreEqual("Index", result.ViewName);
        }

        [Test]
        public void Privacy_ReturnsViewResult()
        {
            // Arrange

            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Privacy_ReturnsView()
        {
            // Arrange

            // Act
            var result = _controller.Privacy() as ViewResult;

            // Assert
            Assert.AreEqual("Privacy", result.ViewName);
        }

        [Test]
        public void Error_ReturnsViewResult()
        {
            // Arrange

            // Act
            var result = _controller.Error();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Error_ReturnsViewWithModel()
        {
            // Arrange

            // Act
            var result = _controller.Error() as ViewResult;
            var model = result.Model as ErrorViewModel;

            // Assert
            Assert.IsNotNull(model);
            Assert.IsNotNull(model.RequestId);
            Assert.AreEqual(Activity.Current?.Id ?? HttpContext.TraceIdentifier, model.RequestId);
        }

        [Test]
        public void Error_LogsError()
        {
            // Arrange
            var exception = new Exception("Test Exception");

            // Act
            var result = _controller.Error();

            // Assert
            _loggerMock.Verify(
                x => x.LogError(exception, "An error occurred while processing your request."),
                Times.Once);
        }
    }
    [TestFixture]
    public class InstallationGuidesTests
    {
        private InstallationGuides _controller;
        private Context _context;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new Context(options);
            _controller = new InstallationGuides(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }

        [Test]
        public async Task Index_ReturnsViewResult()
        {
            // Arrange

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Index_ReturnsViewWithTopics()
        {
            // Arrange
            var topic1 = new Topic { Name = "Topic 1" };
            var topic2 = new Topic { Name = "Topic 2" };
            _context.Topics.Add(topic1);
            _context.Topics.Add(topic2);
            _context.SaveChanges();

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ViewData.Model);
            var model = result.ViewData.Model as List<Topic>;
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count);
            Assert.AreEqual("Topic 1", model[0].Name);
            Assert.AreEqual("Topic 2", model[1].Name);
        }
    }
    [TestClass]
    public class MessagesControllerTests
    {
        private Context _context;
        private MessagesController _controller;

        [TestInitialize]
        public void Initialize()
        {
            // Setup the test database context
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "UserStoryTest")
                .Options;
            _context = new Context(options);

            // Seed test data
            SeedTestData();

            // Setup the controller with the test context
            _controller = new MessagesController(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Cleanup the test database context
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task Index_ReturnsViewResult_WithListOfMessages()
        {
            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var messages = result.Model as List<Message>;
            Assert.IsNotNull(messages);
            Assert.AreEqual(2, messages.Count);
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            // Act
            var result = await _controller.Details(null) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task Details_ReturnsNotFound_WhenMessageIsNull()
        {
            // Arrange
            int messageId = 99;

            // Act
            var result = await _controller.Details(messageId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task Details_ReturnsViewResult_WithMessage()
        {
            // Arrange
            int messageId = 1;

            // Act
            var result = await _controller.Details(messageId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var message = result.Model as Message;
            Assert.IsNotNull(message);
            Assert.AreEqual(messageId, message.ID);
        }

        [TestMethod]
        public void Create_ReturnsViewResult_WithViewData()
        {
            // Act
            var result = _controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ViewData["TopicID"]);
            Assert.IsNotNull(result.ViewData["UserID"]);
        }

        [TestMethod]
        public async Task Create_RedirectsToTopicDetails_WhenModelStateIsValid()
        {
            // Arrange
            int topicId = 1;
            string messageText = "Test message";

            // Act
            var result = await _controller.Create(messageText, topicId) as RedirectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual($"~/Topics/Details/{topicId}", result.Url);

            // Verify that the message was created in the database
            var message = _context.Messages.FirstOrDefault(m => m.MessageText == messageText);
            Assert.IsNotNull(message);
            Assert.AreEqual(topicId, message.TopicID);
        }

        [TestMethod]
        public async Task Edit_ReturnsNotFound_WhenIdIsNull()
        {
            // Act
            var result = await _controller.Edit(null) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

    }
     public class NewsControllerIntegrationTests : IDisposable
    {
        private readonly Context _context;
        private readonly NewsController _controller;

        public NewsControllerIntegrationTests()
        {
            // Set up an in-memory database for testing
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            _context = new Context(options);

            // Seed the database with test data
            SeedDatabase();

            // Set up the NewsController with the test context
            _controller = new NewsController(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithAllNews()
        {
            // Act
            var result = await _controller.Index(null);

            // Assert
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            var model = viewResult.Model as List<News>;
            Assert.Equal(3, model.Count);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithFilteredNews()
        {
            // Act
            var result = await _controller.Index("Politics");

            // Assert
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            var model = viewResult.Model as List<News>;
            Assert.Single(model);
            Assert.Equal("Politics", model[0].Topic);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenNewsDoesNotExist()
        {
            // Act
            var result = await _controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WhenNewsExists()
        {
            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            var model = viewResult.Model as News;
            Assert.Equal(1, model.ID);
        }

        [Fact]
        public async Task Create_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            var news = new News
            {
                Title = "Title",
                Text = "Text",
                Topic = "Topic",
                User = new User(),
                CreationDate = DateTime.Now
            };
            _controller.ModelState.AddModelError("Title", "Required");

            // Act
            var result = await _controller.Create(news);

            // Assert
            Assert.IsType<ViewResult>(result);
        }

    }
    [TestClass]
    public class RolesControllerIntegrationTests
    {
        private readonly DbContextOptions<Context> _options =
            new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "test_database")
                .Options;

        [TestMethod]
        public async Task Index_ReturnsAViewResult_WithAListOfRoles()
        {
            // Arrange
            using (var context = new Context(_options))
            {
                context.Roles.Add(new Role { Name = "Role1" });
                context.Roles.Add(new Role { Name = "Role2" });
                context.SaveChanges();

                var controller = new RolesController(context);

                // Act
                var result = await controller.Index();

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Role>>(
                    viewResult.ViewData.Model);
                Assert.Equal(2, model.Count());
            }
        }

        [TestMethod]
        public async Task Details_ReturnsViewResult_WithRole()
        {
            // Arrange
            using (var context = new Context(_options))
            {
                var role = new Role { Name = "Role1" };
                context.Roles.Add(role);
                context.SaveChanges();

                var controller = new RolesController(context);

                // Act
                var result = await controller.Details(role.ID);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsType<Role>(viewResult.ViewData.Model);
                Assert.Equal(role.ID, model.ID);
                Assert.Equal(role.Name, model.Name);
            }
        }

        [TestMethod]
        public void Details_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            using (var context = new Context(_options))
            {
                var controller = new RolesController(context);

                // Act
                var result = controller.Details(null).Result;

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [TestMethod]
        public async Task Create_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            using (var context = new Context(_options))
            {
                var role = new Role { Name = "Role1" };
                var controller = new RolesController(context);

                // Act
                var result = await controller.Create(role);

                // Assert
                var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", redirectToActionResult.ActionName);

                var rolesInDatabase = await context.Roles.ToListAsync();
                Assert.Equal(1, rolesInDatabase.Count);
                Assert.Equal(role.Name, rolesInDatabase[0].Name);
            }
        }

        [TestMethod]
        public async Task Create_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            using (var context = new Context(_options))
            {
                var role = new Role { Name = "" };
                var controller = new RolesController(context);
                controller.ModelState.AddModelError("Name", "Name is required");

                // Act
                var result = await controller.Create(role);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsType<Role>(viewResult.ViewData.Model);
                Assert.Equal(role.Name, model.Name);
            }
        }
    }
    [TestClass]
    public class TopicsControllerTest
    {
        private Context _context;
        private TopicsController _controller;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new Context(options);

            _controller = new TopicsController(_context);
        }

        [TestMethod]
        public async Task Index_ReturnsViewResult_WithListOfTopics()
        {
            // Arrange
            var topic1 = new Topic { ID = 1, Title = "Topic 1" };
            var topic2 = new Topic { ID = 2, Title = "Topic 2" };

            _context.Topics.Add(topic1);
            _context.Topics.Add(topic2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            var model = viewResult.Model as IQueryable<Topic>;
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count());
        }

        [TestMethod]
        public async Task Details_ReturnsViewResult_WithTopic()
        {
            // Arrange
            var topic = new Topic { ID = 1, Title = "Topic 1" };
            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            var model = viewResult.Model as Topic;
            Assert.IsNotNull(model);
            Assert.AreEqual(topic.ID, model.ID);
            Assert.AreEqual(topic.Title, model.Title);
        }

        [TestMethod]
        public async Task Create_ReturnsViewResult_WithTopic()
        {
            // Act
            var result = await _controller.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            var model = viewResult.Model as Topic;
            Assert.IsNotNull(model);
            Assert.AreEqual(default(int), model.ID);
            Assert.IsNull(model.Title);
        }

        [TestMethod]
        public async Task Create_AddsTopicToDatabase()
        {
            // Arrange
            var user = new User { Login = "testuser" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var topic = new Topic { ID = 1, Title = "Topic 1", UserID = user.ID };

            // Act
            var result = await _controller.Create(topic);

            // Assert
            var topics = _context.Topics.ToList();
            Assert.AreEqual(1, topics.Count());
            var savedTopic = topics.First();
            Assert.AreEqual(topic.ID, savedTopic.ID);
            Assert.AreEqual(topic.Title, savedTopic.Title);
        }
        [Fact]
        public async Task Index_ReturnsViewWithListOfUsers()
        {
          // Arrange
             var options = new DbContextOptionsBuilder<Context>()
           .UseInMemoryDatabase(databaseName: "UserList")
           .Options;
         using (var context = new Context(options))
        {
                context.Users.Add(new User { Login = "testuser1" });
                 context.Users.Add(new User { Login = "testuser2" });
                context.SaveChanges();
            }

                var controller = new UsersController(new Context(options), null);

                // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Model);
            Assert.IsType<List<User>>(result.Model);
            Assert.Equal(2, (result.Model as List<User>).Count);
}

    }

}
