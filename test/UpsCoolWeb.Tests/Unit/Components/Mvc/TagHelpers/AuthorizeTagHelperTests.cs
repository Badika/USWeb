using Microsoft.AspNetCore.Razor.TagHelpers;
using UpsCoolWeb.Components.Security;
using UpsCoolWeb.Tests;
using NSubstitute;
using System;
using Xunit;

namespace UpsCoolWeb.Components.Mvc.Tests
{
    public class AuthorizeTagHelperTests
    {
        private IAuthorization authorization;
        private AuthorizeTagHelper helper;
        private TagHelperOutput output;

        public AuthorizeTagHelperTests()
        {
            output = new TagHelperOutput("authorize", new TagHelperAttributeList(), (useCachedResult, encoder) => null);
            helper = new AuthorizeTagHelper(authorization = Substitute.For<IAuthorization>());
            helper.ViewContext = HtmlHelperFactory.CreateHtmlHelper().ViewContext;
        }

        #region Process(TagHelperContext context, TagHelperOutput output)

        [Fact]
        public void Process_NoAuthorization_RemovedWrappingTag()
        {
            helper = new AuthorizeTagHelper(null);
            helper.ViewContext = HtmlHelperFactory.CreateHtmlHelper().ViewContext;
            helper.ViewContext.HttpContext.User.Identity.Name.Returns("1");

            output.PostContent.SetContent("PostContent");
            output.PostElement.SetContent("PostElement");
            output.PreContent.SetContent("PreContent");
            output.PreElement.SetContent("PreElement");
            output.Content.SetContent("Content");
            output.TagName = "TagName";

            helper.Process(null, output);

            Assert.Equal("PostContent", output.PostContent.GetContent());
            Assert.Equal("PostElement", output.PostElement.GetContent());
            Assert.Equal("PreContent", output.PreContent.GetContent());
            Assert.Equal("PreElement", output.PreElement.GetContent());
            Assert.Equal("Content", output.Content.GetContent());
            Assert.Null(output.TagName);
        }

        [Theory]
        [InlineData("A", "B", "C", "D", "E", "F", "A", "B", "C")]
        [InlineData(null, null, null, "A", "B", "C", "A", "B", "C")]
        [InlineData(null, null, null, null, null, null, null, null, null)]
        public void Process_NotAuthorized_SurpressesOutput(
            String area, String controller, String action,
            String routeArea, String routeController, String routeAction,
            String authArea, String authController, String authAction)
        {
            authorization.IsGrantedFor(1, Arg.Any<String>(), Arg.Any<String>(), Arg.Any<String>()).Returns(true);
            authorization.IsGrantedFor(1, authArea, authController, authAction).Returns(false);
            helper.ViewContext.HttpContext.User.Identity.Name.Returns("1");

            helper.ViewContext.RouteData.Values["controller"] = routeController;
            helper.ViewContext.RouteData.Values["action"] = routeAction;
            helper.ViewContext.RouteData.Values["area"] = routeArea;

            output.PostContent.SetContent("PostContent");
            output.PostElement.SetContent("PostElement");
            output.PreContent.SetContent("PreContent");
            output.PreElement.SetContent("PreElement");
            output.Content.SetContent("Content");
            output.TagName = "TagName";

            helper.Controller = controller;
            helper.Action = action;
            helper.Area = area;

            helper.Process(null, output);

            Assert.Empty(output.PostContent.GetContent());
            Assert.Empty(output.PostElement.GetContent());
            Assert.Empty(output.PreContent.GetContent());
            Assert.Empty(output.PreElement.GetContent());
            Assert.Empty(output.Content.GetContent());
            Assert.Null(output.TagName);
        }

        [Theory]
        [InlineData("A", "B", "C", "D", "E", "F", "A", "B", "C")]
        [InlineData(null, null, null, "A", "B", "C", "A", "B", "C")]
        [InlineData(null, null, null, null, null, null, null, null, null)]
        public void Process_RemovesWrappingTag(
            String area, String controller, String action,
            String routeArea, String routeController, String routeAction,
            String authArea, String authController, String authAction)
        {
            authorization.IsGrantedFor(1, Arg.Any<String>(), Arg.Any<String>(), Arg.Any<String>()).Returns(false);
            authorization.IsGrantedFor(1, authArea, authController, authAction).Returns(true);
            helper.ViewContext.HttpContext.User.Identity.Name.Returns("1");

            helper.ViewContext.RouteData.Values["controller"] = routeController;
            helper.ViewContext.RouteData.Values["action"] = routeAction;
            helper.ViewContext.RouteData.Values["area"] = routeArea;

            output.PostContent.SetContent("PostContent");
            output.PostElement.SetContent("PostElement");
            output.PreContent.SetContent("PreContent");
            output.PreElement.SetContent("PreElement");
            output.Content.SetContent("Content");
            output.TagName = "TagName";

            helper.Controller = controller;
            helper.Action = action;
            helper.Area = area;

            helper.Process(null, output);

            Assert.Equal("PostContent", output.PostContent.GetContent());
            Assert.Equal("PostElement", output.PostElement.GetContent());
            Assert.Equal("PreContent", output.PreContent.GetContent());
            Assert.Equal("PreElement", output.PreElement.GetContent());
            Assert.Equal("Content", output.Content.GetContent());
            Assert.Null(output.TagName);
        }

        #endregion
    }
}
