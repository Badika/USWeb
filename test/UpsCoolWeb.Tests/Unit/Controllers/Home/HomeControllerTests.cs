using Microsoft.AspNetCore.Mvc;
using UpsCoolWeb.Services;
using NSubstitute;
using System;
using Xunit;

namespace UpsCoolWeb.Controllers.Tests
{
    public class HomeControllerTests : ControllerTests
    {
        private HomeController controller;
        private IAccountService service;

        public HomeControllerTests()
        {
            service = Substitute.For<IAccountService>();
            controller = Substitute.ForPartsOf<HomeController>(service);

            ReturnCurrentAccountId(controller, 1);
        }

        #region Index()

        [Fact]
        public void Index_NotActive_RedirectsToLogout()
        {
            service.IsActive(controller.CurrentAccountId).Returns(false);

            Object expected = RedirectToAction(controller, "Logout", "Auth");
            Object actual = controller.Index();

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Index_ReturnsEmptyView()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);

            ViewResult actual = controller.Index() as ViewResult;

            Assert.Null(actual.Model);
        }

        #endregion

        #region Error()

        [Fact]
        public void Error_ReturnsEmptyView()
        {
            ViewResult actual = controller.Error();

            Assert.Null(actual.Model);
        }

        #endregion

        #region NotFound()

        [Fact]
        public void NotFound_NotActive_RedirectsToLogout()
        {
            service.IsLoggedIn(controller.User).Returns(true);
            service.IsActive(controller.CurrentAccountId).Returns(false);

            Object expected = RedirectToAction(controller, "Logout", "Auth");
            Object actual = controller.NotFound();

            Assert.Same(expected, actual);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void NotFound_ReturnsEmptyView(Boolean isLoggedIn, Boolean isActive)
        {
            service.IsActive(controller.CurrentAccountId).Returns(isActive);
            service.IsLoggedIn(controller.User).Returns(isLoggedIn);

            ViewResult actual = controller.NotFound() as ViewResult;

            Assert.Null(actual.Model);
        }

        #endregion
    }
}
