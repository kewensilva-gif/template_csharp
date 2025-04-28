using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using RO.DevTest.Application.Features.Auth.Commands.LoginCommand;
using Microsoft.AspNetCore.Identity;
using DomainUser = RO.DevTest.Domain.Entities.User;
using System.Collections.Generic;
using System.Threading;

namespace RO.DevTest.Tests.Features.Auth
{
    public class LoginCommandHandlerTests
    {
        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var user = new DomainUser
            {
                Id = "123",
                UserName = "testuser",
                Email = "testuser@example.com"
            };

            var userManagerMock = new Mock<UserManager<DomainUser>>(
                Mock.Of<IUserStore<DomainUser>>(), null, null, null, null, null, null, null, null
            );
            var signInManagerMock = new Mock<SignInManager<DomainUser>>(
                userManagerMock.Object,
                Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<DomainUser>>(),
                null, null, null, null
            );

            userManagerMock.Setup(x => x.FindByNameAsync("testuser"))
                .ReturnsAsync(user);
            signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, "password", false))
                .ReturnsAsync(SignInResult.Success);
            userManagerMock.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Admin" });

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(x => x["Jwt:Key"]).Returns("super_secret_key_that_is_long_enough_to_be_valid_1234");
            configMock.Setup(x => x["Jwt:Issuer"]).Returns("test.issuer");
            configMock.Setup(x => x["Jwt:Audience"]).Returns("test.audience");

            var handler = new LoginCommandHandler(
                userManagerMock.Object,
                signInManagerMock.Object,
                configMock.Object
            );

            var command = new LoginCommand
            {
                Username = "testuser",
                Password = "password"
            };

            var result = await handler.Handle(command, CancellationToken.None);

            result.AccessToken.Should().NotBeNullOrWhiteSpace();
            result.Roles.Should().Contain("Admin");
        }
    }
}
