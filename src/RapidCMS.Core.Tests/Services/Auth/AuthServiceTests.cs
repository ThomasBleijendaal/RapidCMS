using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Components.Authorization;
using Moq;
using NUnit.Framework;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Authorization;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Services.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace RapidCMS.Core.Tests.Services.Auth
{
    public class AuthServiceTests
    {
        private IAuthService _subject = default!;

        private Mock<IServiceProvider> _serviceProvider = default!;
        private Mock<IAuthorizationService> _authorizationService = default!;
        private Mock<IButtonActionHandler> _buttonActionHandler = default!;
        private Mock<IButtonActionHandlerResolver> _buttonActionHandlerResolver = default!;
        private Mock<AuthenticationStateProvider> _authenticationStateProvider = default!;
        private readonly ClaimsPrincipal _user = new ClaimsPrincipal();

        [SetUp]
        public void Setup()
        {
            _authorizationService = new Mock<IAuthorizationService>();

            _serviceProvider = new Mock<IServiceProvider>();
            _serviceProvider
                .Setup(x => x.GetService(It.Is<Type>(x => x == typeof(IAuthorizationService))))
                .Returns(_authorizationService.Object);

            _buttonActionHandler = new Mock<IButtonActionHandler>();
            _buttonActionHandlerResolver = new Mock<IButtonActionHandlerResolver>();
            _buttonActionHandlerResolver
                .Setup(x => x.GetButtonActionHandler(It.IsAny<IButtonSetup>()))
                .Returns(_buttonActionHandler.Object);

            var state = new AuthenticationState(_user);

            _authenticationStateProvider = new Mock<AuthenticationStateProvider>();
            _authenticationStateProvider.Setup(x => x.GetAuthenticationStateAsync()).ReturnsAsync(state);

            _subject = new ServerSideAuthService(
                _buttonActionHandlerResolver.Object,
                _authenticationStateProvider.Object,
                _serviceProvider.Object);
        }

        [TestCase(UsageType.Add, "Add")]
        [TestCase(UsageType.Edit, "Update")]
        [TestCase(UsageType.New, "Create")]
        [TestCase(UsageType.Pick, "Add")]
        [TestCase(UsageType.View, "Read")]
        public void WhenCheckingIfUsageTypeIsAllowedOnEntity_AuthorizationServiceShouldBeConsulted(UsageType usageType, string requirement)
        {
            // arrange
            var entity = new Mock<IEntity>();

            // act
            _subject.EnsureAuthorizedUserAsync(usageType, entity.Object);

            // assert
            _authenticationStateProvider.Verify(x => x.GetAuthenticationStateAsync(), Times.Once());
            _authenticationStateProvider.VerifyNoOtherCalls();
            _authorizationService.Verify(x => x.AuthorizeAsync(
                It.Is<ClaimsPrincipal>(x => x == _user),
                It.Is<IEntity>(x => x == entity.Object),
                It.Is<IEnumerable<IAuthorizationRequirement>>(x => (x.First() as OperationAuthorizationRequirement)!.Name == requirement)));
            _authorizationService.VerifyNoOtherCalls();
        }

        [TestCase(UsageType.Add, "Add")]
        [TestCase(UsageType.Edit, "Update")]
        [TestCase(UsageType.New, "Create")]
        [TestCase(UsageType.Pick, "Add")]
        [TestCase(UsageType.View, "Read")]
        public void WhenUsageTypeIsNotAllowed_AuthServiceShouldThrowUnauthorizedAccessException(UsageType usageType, string requirement)
        {
            // arrange
            var entity = new Mock<IEntity>();
            _authorizationService
                .Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<IEntity>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Failed());

            // act & assert
            Assert.ThrowsAsync(typeof(UnauthorizedAccessException), () => _subject.EnsureAuthorizedUserAsync(usageType, entity.Object));
        }

        [TestCase(UsageType.Add, "Add")]
        [TestCase(UsageType.Edit, "Update")]
        [TestCase(UsageType.New, "Create")]
        [TestCase(UsageType.Pick, "Add")]
        [TestCase(UsageType.View, "Read")]
        public void WhenCheckingButtonIsAllowed_AuthorizationServiceShouldBeConsulted(UsageType usageType, string requirement)
        {
            // arrange
            _buttonActionHandler.Setup(x => x.GetOperation(It.IsAny<IButton>(), It.IsAny<EditContext>())).Returns(Operations.GetOperationForUsageType(usageType));
            var serviceProvider = new Mock<IServiceProvider>();
            var entity = new Mock<IEntity>();
            var editContext = new EditContext("alias", "repo", "entity", entity.Object, default, usageType, serviceProvider.Object);
            var button = new ButtonSetup();

            // act
            _subject.EnsureAuthorizedUserAsync(editContext, button);

            // assert
            _authenticationStateProvider.Verify(x => x.GetAuthenticationStateAsync(), Times.Once());
            _authenticationStateProvider.VerifyNoOtherCalls();
            _authorizationService.Verify(x => x.AuthorizeAsync(
                It.Is<ClaimsPrincipal>(x => x == _user),
                It.Is<IEntity>(x => x == entity.Object),
                It.Is<IEnumerable<IAuthorizationRequirement>>(x => (x.First() as OperationAuthorizationRequirement)!.Name == requirement)));
            _authorizationService.VerifyNoOtherCalls();
        }

        [TestCase(UsageType.Add, "Add")]
        [TestCase(UsageType.Edit, "Update")]
        [TestCase(UsageType.New, "Create")]
        [TestCase(UsageType.Pick, "Add")]
        [TestCase(UsageType.View, "Read")]
        public void WhenButtonIsNotAllowed_AuthServiceShouldThrowUnauthorizedAccessException(UsageType usageType, string requirement)
        {
            // arrange
            _buttonActionHandler.Setup(x => x.GetOperation(It.IsAny<IButton>(), It.IsAny<EditContext>())).Returns(Operations.GetOperationForUsageType(usageType));
            var serviceProvider = new Mock<IServiceProvider>();
            var entity = new Mock<IEntity>();
            var editContext = new EditContext("alias", "repo", "entity", entity.Object, default, usageType, serviceProvider.Object);
            var button = new ButtonSetup();
            _authorizationService
                .Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<IEntity>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Failed());

            // act & assert
            Assert.ThrowsAsync(typeof(UnauthorizedAccessException), () => _subject.EnsureAuthorizedUserAsync(editContext, button));
        }
    }
}
