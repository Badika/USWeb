using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using UpsCoolWeb.Components.Notifications;
using UpsCoolWeb.Components.Security;
using UpsCoolWeb.Objects;
using UpsCoolWeb.Resources;
using UpsCoolWeb.Services;
using UpsCoolWeb.Tests;
using UpsCoolWeb.Validators;
using NSubstitute;
using System;
using System.Linq;
using Xunit;

namespace UpsCoolWeb.Controllers.Tests
{
    public class ProfileControllerTests : ControllerTests
    {
        private ProfileDeleteView profileDelete;
        private ProfileController controller;
        private ProfileEditView profileEdit;
        private IAccountValidator validator;
        private IAccountService service;

        public ProfileControllerTests()
        {
            validator = Substitute.For<IAccountValidator>();
            service = Substitute.For<IAccountService>();

            profileDelete = ObjectsFactory.CreateProfileDeleteView();
            profileEdit = ObjectsFactory.CreateProfileEditView();

            controller = Substitute.ForPartsOf<ProfileController>(validator, service);
            controller.ControllerContext.HttpContext = Substitute.For<HttpContext>();
            controller.TempData = Substitute.For<ITempDataDictionary>();
            controller.ControllerContext.RouteData = new RouteData();
            controller.Url = Substitute.For<IUrlHelper>();
            ReturnCurrentAccountId(controller, 1);
        }

        #region Edit()

        [Fact]
        public void Edit_NotActive_RedirectsToLogout()
        {
            service.IsActive(controller.CurrentAccountId).Returns(false);

            Object expected = RedirectToAction(controller, "Logout", "Auth");
            Object actual = controller.Edit();

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Edit_ReturnsProfileView()
        {
            service.Get<ProfileEditView>(controller.CurrentAccountId).Returns(profileEdit);
            service.IsActive(controller.CurrentAccountId).Returns(true);

            Object actual = (controller.Edit() as ViewResult).Model;
            Object expected = profileEdit;

            Assert.Same(expected, actual);
        }

        #endregion

        #region Edit(ProfileEditView profile)

        [Fact]
        public void Edit_ProtectsFromOverpostingId()
        {
            ProtectsFromOverpostingId(controller, "Edit");
        }

        [Fact]
        public void Edit_Post_NotActive_RedirectsToLogout()
        {
            service.IsActive(controller.CurrentAccountId).Returns(false);

            Object expected = RedirectToAction(controller, "Logout", "Auth");
            Object actual = controller.Edit(null);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Edit_CanNotEdit_ReturnsSameView()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);
            validator.CanEdit(profileEdit).Returns(false);

            Object actual = (controller.Edit(profileEdit) as ViewResult).Model;
            Object expected = profileEdit;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Edit_Profile()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);
            validator.CanEdit(profileEdit).Returns(true);

            controller.Edit(profileEdit);

            service.Received().Edit(profileEdit);
        }

        [Fact]
        public void Edit_AddsUpdatedMessage()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);
            validator.CanEdit(profileEdit).Returns(true);

            controller.Edit(profileEdit);

            Alert actual = controller.Alerts.Single();

            Assert.Equal(Message.For<AccountView>("ProfileUpdated"), actual.Message);
            Assert.Equal(AlertType.Success, actual.Type);
            Assert.Equal(4000, actual.Timeout);
        }

        [Fact]
        public void Edit_RedirectsToEdit()
        {
            validator.CanEdit(profileEdit).Returns(true);
            service.IsActive(controller.CurrentAccountId).Returns(true);

            Object expected = RedirectToAction(controller, "Edit");
            Object actual = controller.Edit(profileEdit);

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Delete()

        [Fact]
        public void Delete_NotActive_RedirectsToLogout()
        {
            service.IsActive(controller.CurrentAccountId).Returns(false);

            Object expected = RedirectToAction(controller, "Logout", "Auth");
            Object actual = controller.Delete();

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Delete_AddsDisclaimerMessage()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);

            controller.Delete();

            Alert actual = controller.Alerts.Single();

            Assert.Equal(Message.For<AccountView>("ProfileDeleteDisclaimer"), actual.Message);
            Assert.Equal(AlertType.Warning, actual.Type);
            Assert.Equal(0, actual.Timeout);
        }

        [Fact]
        public void Delete_ReturnsEmptyView()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);

            ViewResult actual = controller.Delete() as ViewResult;

            Assert.Null(actual.Model);
        }

        #endregion

        #region DeleteConfirmed(ProfileDeleteView profile)

        [Fact]
        public void DeleteConfirmed_ProtectsFromOverpostingId()
        {
            ProtectsFromOverpostingId(controller, "DeleteConfirmed");
        }

        [Fact]
        public void DeleteConfirmed_NotActive_RedirectsToLogout()
        {
            service.IsActive(controller.CurrentAccountId).Returns(false);

            Object expected = RedirectToAction(controller, "Logout", "Auth");
            Object actual = controller.DeleteConfirmed(profileDelete);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void DeleteConfirmed_CanNotDelete_AddsDisclaimerMessage()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);
            validator.CanDelete(profileDelete).Returns(false);

            controller.DeleteConfirmed(profileDelete);

            Alert actual = controller.Alerts.Single();

            Assert.Equal(Message.For<AccountView>("ProfileDeleteDisclaimer"), actual.Message);
            Assert.Equal(AlertType.Warning, actual.Type);
            Assert.Equal(0, actual.Timeout);
        }

        [Fact]
        public void DeleteConfirmed_CanNotDelete_ReturnsEmptyView()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);
            validator.CanDelete(profileDelete).Returns(false);

            ViewResult actual = controller.DeleteConfirmed(profileDelete) as ViewResult;

            Assert.Null(actual.Model);
        }

        [Fact]
        public void DeleteConfirmed_DeletesProfile()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);
            validator.CanDelete(profileDelete).Returns(true);

            controller.DeleteConfirmed(profileDelete);

            service.Received().Delete(controller.CurrentAccountId);
        }

        [Fact]
        public void DeleteConfirmed_RefreshesAuthorization()
        {
            controller.HttpContext.RequestServices.GetService<IAuthorization>().Returns(Substitute.For<IAuthorization>());
            service.IsActive(controller.CurrentAccountId).Returns(true);
            validator.CanDelete(profileDelete).Returns(true);
            controller.OnActionExecuting(null);

            controller.DeleteConfirmed(profileDelete);

            controller.Authorization.Received().Refresh();
        }

        [Fact]
        public void DeleteConfirmed_RedirectsToAuthLogout()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);
            validator.CanDelete(profileDelete).Returns(true);

            Object expected = RedirectToAction(controller, "Logout", "Auth");
            Object actual = controller.DeleteConfirmed(profileDelete);

            Assert.Same(expected, actual);
        }

        #endregion
    }
}
