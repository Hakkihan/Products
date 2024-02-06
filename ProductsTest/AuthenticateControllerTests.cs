using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Products.Auth;
using Products.Controllers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsTest
{
    internal class AuthenticateControllerTests
    {

        private AuthenticateController _authenticateController;
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private Mock<IConfiguration> _mockConfiguration;

        [SetUp]
        public void Setup()
        {
            _mockUserManager = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["JWT:Secret"]).Returns("JWTMockSecretMockForTestingPurposes12345");
            _mockConfiguration.Setup(c => c["JWT:ValidIssuer"]).Returns("https://localhost:7148");
            _mockConfiguration.Setup(c => c["JWT:ValidAudience"]).Returns("https://localhost:4200");

            _authenticateController = new AuthenticateController(_mockUserManager.Object, _mockRoleManager.Object, _mockConfiguration.Object);
        }

        [Test]
        public async Task Login_ValidCredentials_CallsJWT()
        {
            var validModel = new LoginModel { Username = "newUser", Password = "Password123!" };
            IdentityUser user = new IdentityUser { UserName = validModel.Username };
            var userRoles = new UserRoles();
            
            _mockUserManager.Setup(m => m.FindByNameAsync(validModel.Username)).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.CheckPasswordAsync(user, validModel.Password)).ReturnsAsync(true);
            _mockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

            // Act
            var result = await _authenticateController.Login(validModel) as ObjectResult;

            // Assert

            _mockConfiguration.Verify(c => c["JWT:Secret"], Times.Once);
            _mockConfiguration.Verify(c => c["JWT:ValidIssuer"], Times.Once);
            _mockConfiguration.Verify(c => c["JWT:ValidAudience"], Times.Once);
        }



        [Test]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var invalidModel = new LoginModel { Username = "invalidUser", Password = "invalidPassword" };
            _mockUserManager.Setup(m => m.FindByNameAsync(invalidModel.Username)).ReturnsAsync((IdentityUser)null);

            // Act
            var result = await _authenticateController.Login(invalidModel) as UnauthorizedResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(401));
        }

        [Test]
        public async Task Register_ValidModel_ReturnsSuccess()
        {
            // Arrange
            var validModel = new RegisterModel { Username = "newUser", EmailAddress = "user@example.com", Password = "Password123" };
            _mockUserManager.Setup(m => m.FindByNameAsync(validModel.Username)).ReturnsAsync((IdentityUser)null);
            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), validModel.Password)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _authenticateController.Register(validModel) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            var response = result.Value as Response;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Success"));
            Assert.That(response.Message, Is.EqualTo("User created."));
        }

        [Test]
        public async Task Register_DuplicateUser_ReturnsInternalServerError()
        {
            // Arrange
            var duplicateModel = new RegisterModel { Username = "existingUser", EmailAddress = "user@example.com", Password = "Password123" };
            _mockUserManager.Setup(m => m.FindByNameAsync(duplicateModel.Username)).ReturnsAsync(new IdentityUser());

            // Act
            var result = await _authenticateController.Register(duplicateModel) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            var response = result.Value as Response;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("User already exists"));
        }

        [Test]
        public async Task Register_InvalidPassword_ReturnsInternalServerError()
        {
            // Arrange
            var invalidPasswordModel = new RegisterModel { Username = "newUser", EmailAddress = "user@example.com", Password = "weak" };
            _mockUserManager.Setup(m => m.FindByNameAsync(invalidPasswordModel.Username)).ReturnsAsync((IdentityUser)null);
            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), invalidPasswordModel.Password)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password requirements not met" }));

            // Act
            var result = await _authenticateController.Register(invalidPasswordModel) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            var response = result.Value as Response;
            Assert.IsNotNull(response);
            Assert.That(response.Status, Is.EqualTo("Error"));
            Assert.That(response.Message, Is.EqualTo("User creation failed. Please make sure the username contains only a-z,A-Z and the password contains non-alphanumeric characters."));
        }
    }
}
