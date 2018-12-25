using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using UpsCoolWeb.Components.Notifications;
using UpsCoolWeb.Components.Security;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace UpsCoolWeb.Controllers.Tests
{
    public class BaseControllerTests : ControllerTests
    {
        private BaseController controller;
        private String controllerName;
        private ActionContext action;
        private String areaName;

        public BaseControllerTests()
        {
            controller = Substitute.ForPartsOf<BaseController>();

            controller.Url = Substitute.For<IUrlHelper>();
            controller.ControllerContext.RouteData = new RouteData();
            controller.TempData = Substitute.For<ITempDataDictionary>();
            controller.ControllerContext.HttpContext = Substitute.For<HttpContext>();
            controller.HttpContext.RequestServices.GetService<IAuthorization>().Returns(Substitute.For<IAuthorization>());

            action = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());

            controllerName = controller.RouteData.Values["controller"] as String;
            areaName = controller.RouteData.Values["area"] as String;
        }

        #region BaseController()

        [Fact]
        public void BaseController_CreatesEmptyAlerts()
        {
            Assert.Empty(controller.Alerts);
        }

        #endregion

        #region NotEmptyView(Object model)

        [Fact]
        public void NotEmptyView_NullModel_RedirectsToNotFound()
        {
            Object expected = RedirectToNotFound(controller);
            Object actual = controller.NotEmptyView(null);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void NotEmptyView_ReturnsModelView()
        {
            Object expected = new Object();
            Object actual = (controller.NotEmptyView(expected) as ViewResult).Model;

            Assert.Same(expected, actual);
        }

        #endregion

        #region RedirectToLocal(String url)

        [Fact]
        public void RedirectToLocal_NotLocalUrl_RedirectsToDefault()
        {
            controller.Url.IsLocalUrl("T").Returns(false);

            Object expected = RedirectToDefault(controller);
            Object actual = controller.RedirectToLocal("T");

            Assert.Same(expected, actual);
        }

        [Fact]
        public void RedirectToLocal_IsLocalUrl_RedirectsToLocal()
        {
            controller.Url.IsLocalUrl("/").Returns(true);

            String actual = (controller.RedirectToLocal("/") as RedirectResult).Url;
            String expected = "/";

            Assert.Equal(expected, actual);
        }

        #endregion

        #region RedirectToDefault()

        [Fact]
        public void RedirectToDefault_Route()
        {
            RedirectToActionResult actual = controller.RedirectToDefault();

            Assert.Equal("", actual.RouteValues["area"]);
            Assert.Equal("Home", actual.ControllerName);
            Assert.Equal("Index", actual.ActionName);
            Assert.Single(actual.RouteValues);
        }

        #endregion

        #region RedirectToNotFound()

        [Fact]
        public void RedirectToNotFound_Route()
        {
            RedirectToActionResult actual = controller.RedirectToNotFound();

            Assert.Equal("", actual.RouteValues["area"]);
            Assert.Equal("NotFound", actual.ActionName);
            Assert.Equal("Home", actual.ControllerName);
            Assert.Single(actual.RouteValues);
        }

        #endregion

        #region RedirectToAction(String action, String controller, Object route)

        [Fact]
        public void RedirectToAction_Action_Controller_Route_NotAuthorized_RedirectsToDefault()
        {
            controller.IsAuthorizedFor("Action", "Controller", areaName).Returns(false);

            Object expected = RedirectToDefault(controller);
            Object actual = controller.RedirectToAction("Action", "Controller", new { id = "Id" });

            Assert.Same(expected, actual);
        }

        [Fact]
        public void RedirectToAction_Action_NullController_NullRoute_RedirectsToAction()
        {
            controller.IsAuthorizedFor("Action", controllerName, areaName).Returns(true);

            RedirectToActionResult actual = controller.RedirectToAction("Action", null, null);

            Assert.Equal(controllerName, actual.ControllerName);
            Assert.Equal("Action", actual.ActionName);
            Assert.Null(actual.RouteValues);
        }

        [Fact]
        public void RedirectToAction_Action_Controller_NullRoute_RedirectsToAction()
        {
            controller.IsAuthorizedFor("Action", "Controller", areaName).Returns(true);

            RedirectToActionResult actual = controller.RedirectToAction("Action", "Controller", null);

            Assert.Equal("Controller", actual.ControllerName);
            Assert.Equal("Action", actual.ActionName);
            Assert.Null(actual.RouteValues);
        }

        [Fact]
        public void RedirectToAction_Action_Controller_Route_RedirectsToAction()
        {
            controller.IsAuthorizedFor("Action", "Controller", "Area").Returns(true);

            RedirectToActionResult actual = controller.RedirectToAction("Action", "Controller", new { area = "Area", id = "Id" });

            Assert.Equal("Controller", actual.ControllerName);
            Assert.Equal("Area", actual.RouteValues["area"]);
            Assert.Equal("Id", actual.RouteValues["id"]);
            Assert.Equal("Action", actual.ActionName);
            Assert.Equal(2, actual.RouteValues.Count);
        }

        #endregion

        #region IsAuthorizedFor(String action, String controller, String area)

        [Fact]
        public void IsAuthorizedFor_NoAuthorization_ReturnsTrue()
        {
            controller = Substitute.ForPartsOf<BaseController>();

            Assert.Null(controller.Authorization);
            Assert.True(controller.IsAuthorizedFor(null, null, null));
        }

        [Fact]
        public void IsAuthorizedFor_ReturnsAuthorizationResult()
        {
            IAuthorization authorization = controller.HttpContext.RequestServices.GetService<IAuthorization>();
            authorization.IsGrantedFor(controller.CurrentAccountId, "Area", "Controller", "Action").Returns(true);

            controller.OnActionExecuting(null);

            Assert.True(controller.IsAuthorizedFor("Action", "Controller", "Area"));
            Assert.Same(authorization, controller.Authorization);
        }

        #endregion

        #region OnActionExecuting(ActionExecutingContext context)

        [Fact]
        public void OnActionExecuting_SetsAuthorization()
        {
            IAuthorization authorization = controller.HttpContext.RequestServices.GetService<IAuthorization>();

            controller.OnActionExecuting(null);

            Object actual = controller.Authorization;
            Object expected = authorization;

            Assert.Same(expected, actual);
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("1", 1)]
        [InlineData(null, 0)]
        public void OnActionExecuting_SetsCurrentAccountId(String identity, Int32 accountId)
        {
            controller.HttpContext.User.Identity.Name.Returns(identity);

            controller.OnActionExecuting(null);

            Int32? actual = controller.CurrentAccountId;
            Int32? expected = accountId;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region OnActionExecuted(ActionExecutedContext context)

        [Fact]
        public void OnActionExecuted_JsonResult_NoAlerts()
        {
            controller.Alerts.AddError("Test");
            controller.TempData["Alerts"] = null;
            JsonResult result = new JsonResult("Value");

            controller.OnActionExecuted(new ActionExecutedContext(action, new List<IFilterMetadata>(), null) { Result = result });

            Assert.Null(controller.TempData["Alerts"]);
        }

        [Fact]
        public void OnActionExecuted_NullTempDataAlerts_SetsTempDataAlerts()
        {
            controller.Alerts.AddError("Test");
            controller.TempData["Alerts"] = null;

            controller.OnActionExecuted(new ActionExecutedContext(action, new List<IFilterMetadata>(), null));

            Object expected = JsonConvert.SerializeObject(controller.Alerts);
            Object actual = controller.TempData["Alerts"];

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OnActionExecuted_MergesTempDataAlerts()
        {
            Alerts alerts = new Alerts();
            alerts.AddError("Test1");

            controller.TempData["Alerts"] = JsonConvert.SerializeObject(alerts);

            controller.Alerts.AddError("Test2");
            alerts.AddError("Test2");

            controller.OnActionExecuted(new ActionExecutedContext(action, new List<IFilterMetadata>(), null));

            Object expected = JsonConvert.SerializeObject(alerts);
            Object actual = controller.TempData["Alerts"];

            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
