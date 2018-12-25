using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using UpsCoolWeb.Components.Mail;
using UpsCoolWeb.Components.Notifications;
using UpsCoolWeb.Objects;
using UpsCoolWeb.Resources;
using UpsCoolWeb.Services;
using UpsCoolWeb.Tests;
using UpsCoolWeb.Validators;
using NSubstitute;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UpsCoolWeb.Controllers.Tests
{
    public class AuthControllerTests : ControllerTests
    {
        private AccountRecoveryView accountRecovery;
        private AccountResetView accountReset;
        private AccountLoginView accountLogin;
        private IAccountValidator validator;
        private AuthController controller;
        private IAccountService service;
        private IMailClient mail;

        public AuthControllerTests()
        {
            mail = Substitute.For<IMailClient>();
            service = Substitute.For<IAccountService>();
            validator = Substitute.For<IAccountValidator>();
            controller = Substitute.ForPartsOf<AuthController>(validator, service, mail);
            controller.ControllerContext.HttpContext = Substitute.For<HttpContext>();
            controller.TempData = Substitute.For<ITempDataDictionary>();
            controller.ControllerContext.RouteData = new RouteData();
            controller.Url = Substitute.For<IUrlHelper>();

            accountRecovery = ObjectsFactory.CreateAccountRecoveryView();
            accountReset = ObjectsFactory.CreateAccountResetView();
            accountLogin = ObjectsFactory.CreateAccountLoginView();
        }

        #region Recover()

        [Fact]
        public void Recover_IsLoggedIn_RedirectsToDefault()
        {
            service.IsLoggedIn(controller.User).Returns(true);

            Object expected = RedirectToDefault(controller);
            Object actual = controller.Recover();

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Recover_ReturnsEmptyView()
        {
            service.IsLoggedIn(controller.User).Returns(false);

            ViewResult actual = controller.Recover() as ViewResult;

            Assert.Null(actual.Model);
        }

        #endregion

        #region Recover(AccountRecoveryView account)

        [Fact]
        public async Task Recover_Post_IsLoggedIn_RedirectsToDefault()
        {
            service.IsLoggedIn(controller.User).Returns(true);
            validator.CanRecover(accountRecovery).Returns(true);

            Object expected = RedirectToDefault(controller);
            Object actual = await controller.Recover(null);

            Assert.Same(expected, actual);
        }

        [Fact]
        public async Task Recover_CanNotRecover_ReturnsSameView()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanRecover(accountRecovery).Returns(false);

            Object actual = (await controller.Recover(accountRecovery) as ViewResult).Model;
            Object expected = accountRecovery;

            Assert.Same(expected, actual);
        }

        [Fact]
        public async Task Recover_Account()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanRecover(accountRecovery).Returns(true);

            await controller.Recover(accountRecovery);

            service.Received().Recover(accountRecovery);
        }

        [Fact]
        public async Task Recover_SendsRecoveryInformation()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanRecover(accountRecovery).Returns(true);
            service.Recover(accountRecovery).Returns("TestToken");

            await controller.Recover(accountRecovery);

            String url = controller.Url.Action("Reset", "Auth", new { token = "TestToken" }, controller.Request.Scheme);
            String subject = Message.For<AccountView>("RecoveryEmailSubject");
            String body = Message.For<AccountView>("RecoveryEmailBody", url);
            String email = accountRecovery.Email;

            await mail.Received().SendAsync(email, subject, body);
        }

        [Fact]
        public async Task Recover_NullToken_DoesNotSendRecoveryInformation()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanRecover(accountRecovery).Returns(true);
            service.Recover(accountRecovery).Returns((String)null);

            await controller.Recover(accountRecovery);

            await mail.DidNotReceive().SendAsync(Arg.Any<String>(), Arg.Any<String>(), Arg.Any<String>());
        }

        [Fact]
        public async Task Recover_AddsRecoveryMessage()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanRecover(accountRecovery).Returns(true);
            service.Recover(accountRecovery).Returns("RecoveryToken");

            await controller.Recover(accountRecovery);

            Alert actual = controller.Alerts.Single();

            Assert.Equal(Message.For<AccountView>("RecoveryInformation"), actual.Message);
            Assert.Equal(AlertType.Info, actual.Type);
            Assert.Equal(0, actual.Timeout);
        }

        [Fact]
        public async Task Recover_RedirectsToLogin()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanRecover(accountRecovery).Returns(true);
            service.Recover(accountRecovery).Returns("RecoveryToken");

            Object expected = RedirectToAction(controller, "Login");
            Object actual = await controller.Recover(accountRecovery);

            Assert.Same(expected, actual);
        }

        #endregion

        #region Reset(String token)

        [Fact]
        public void Reset_IsLoggedIn_RedirectsToDefault()
        {
            service.IsLoggedIn(controller.User).Returns(true);

            Object expected = RedirectToDefault(controller);
            Object actual = controller.Reset("");

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Reset_CanNotReset_RedirectsToRecover()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanReset(Arg.Any<AccountResetView>()).Returns(false);

            Object expected = RedirectToAction(controller, "Recover");
            Object actual = controller.Reset("Token");

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Reset_ReturnsEmptyView()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanReset(Arg.Any<AccountResetView>()).Returns(true);

            ViewResult actual = controller.Reset("") as ViewResult;

            Assert.Null(actual.Model);
        }

        #endregion

        #region Reset(AccountResetView account)

        [Fact]
        public void Reset_Post_IsLoggedIn_RedirectsToDefault()
        {
            service.IsLoggedIn(controller.User).Returns(true);

            Object expected = RedirectToDefault(controller);
            Object actual = controller.Reset(accountReset);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Reset_Post_CanNotReset_RedirectsToRecover()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanReset(accountReset).Returns(false);

            Object expected = RedirectToAction(controller, "Recover");
            Object actual = controller.Reset(accountReset);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Reset_Account()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanReset(accountReset).Returns(true);

            controller.Reset(accountReset);

            service.Received().Reset(accountReset);
        }

        [Fact]
        public void Reset_AddsResetMessage()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanReset(accountReset).Returns(true);

            controller.Reset(accountReset);

            Alert actual = controller.Alerts.Single();

            Assert.Equal(Message.For<AccountView>("SuccessfulReset"), actual.Message);
            Assert.Equal(AlertType.Success, actual.Type);
            Assert.Equal(4000, actual.Timeout);
        }

        [Fact]
        public void Reset_RedirectsToLogin()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanReset(accountReset).Returns(true);

            Object expected = RedirectToAction(controller, "Login");
            Object actual = controller.Reset(accountReset);

            Assert.Same(expected, actual);
        }

        #endregion

        #region Login(String returnUrl)

        [Fact]
        public void Login_IsLoggedIn_RedirectsToUrl()
        {
            service.IsLoggedIn(controller.User).Returns(true);
            controller.When(sub => sub.RedirectToLocal("/")).DoNotCallBase();
            controller.RedirectToLocal("/").Returns(new RedirectResult("/"));

            Object expected = controller.RedirectToLocal("/");
            Object actual = controller.Login("/");

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Login_NotLoggedIn_ReturnsEmptyView()
        {
            service.IsLoggedIn(controller.User).Returns(false);

            ViewResult actual = controller.Login("/") as ViewResult;

            Assert.Null(actual.Model);
        }

        #endregion

        #region Login(AccountLoginView account, String returnUrl)

        [Fact]
        public void Login_Post_IsLoggedIn_RedirectsToUrl()
        {
            service.IsLoggedIn(controller.User).Returns(true);
            controller.When(sub => sub.RedirectToLocal("/")).DoNotCallBase();
            controller.RedirectToLocal("/").Returns(new RedirectResult("/"));

            Object expected = controller.RedirectToLocal("/");
            Object actual = controller.Login(null, "/");

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Login_CanNotLogin_ReturnsSameView()
        {
            validator.CanLogin(accountLogin).Returns(false);

            Object actual = (controller.Login(accountLogin, null) as ViewResult).Model;
            Object expected = accountLogin;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Login_Account()
        {
            validator.CanLogin(accountLogin).Returns(true);
            controller.When(sub => sub.RedirectToLocal(null)).DoNotCallBase();
            controller.RedirectToLocal(null).Returns(new RedirectResult("/"));

            controller.Login(accountLogin, null);

            service.Received().Login(controller.HttpContext, accountLogin.Username);
        }

        [Fact]
        public void Login_RedirectsToUrl()
        {
            validator.CanLogin(accountLogin).Returns(true);
            controller.When(sub => sub.RedirectToLocal("/")).DoNotCallBase();
            controller.RedirectToLocal("/").Returns(new RedirectResult("/"));

            Object actual = controller.Login(accountLogin, "/");
            Object expected = controller.RedirectToLocal("/");

            Assert.Same(expected, actual);
        }

        #endregion

        #region Logout()

        [Fact]
        public void Logout_Account()
        {
            controller.Logout();

            service.Received().Logout(controller.HttpContext);
        }

        [Fact]
        public void Logout_RedirectsToLogin()
        {
            Object expected = RedirectToAction(controller, "Login");
            Object actual = controller.Logout();

            Assert.Same(expected, actual);
        }

        #endregion
    }
}
