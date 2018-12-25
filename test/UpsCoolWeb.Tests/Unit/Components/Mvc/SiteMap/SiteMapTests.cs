using Microsoft.AspNetCore.Mvc.Rendering;
using UpsCoolWeb.Components.Extensions;
using UpsCoolWeb.Components.Security;
using UpsCoolWeb.Tests;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace UpsCoolWeb.Components.Mvc.Tests
{
    public class SiteMapTests
    {
        private IDictionary<String, Object> route;
        private IAuthorization authorization;
        private ViewContext context;
        private SiteMap siteMap;

        public SiteMapTests()
        {
            authorization = Substitute.For<IAuthorization>();
            siteMap = new SiteMap(CreateSiteMap(), authorization);

            context = HtmlHelperFactory.CreateHtmlHelper().ViewContext;
            route = context.RouteData.Values;
        }

        #region For(ViewContext context)

        [Fact]
        public void For_NoAuthorization_ReturnsAllNodes()
        {
            siteMap = new SiteMap(CreateSiteMap(), null);

            SiteMapNode[] actual = siteMap.For(context).ToArray();

            Assert.Single(actual);

            Assert.Null(actual[0].Action);
            Assert.Null(actual[0].Controller);
            Assert.Equal("Administration", actual[0].Area);
            Assert.Equal("fa fa-cogs", actual[0].IconClass);

            actual = actual[0].Children.ToArray();

            Assert.Equal(2, actual.Length);

            Assert.Empty(actual[0].Children);

            Assert.Equal("Index", actual[0].Action);
            Assert.Equal("Accounts", actual[0].Controller);
            Assert.Equal("Administration", actual[0].Area);
            Assert.Equal("fa fa-user", actual[0].IconClass);

            Assert.Null(actual[1].Action);
            Assert.Equal("Roles", actual[1].Controller);
            Assert.Equal("Administration", actual[1].Area);
            Assert.Equal("fa fa-users", actual[1].IconClass);

            actual = actual[1].Children.ToArray();

            Assert.Single(actual);
            Assert.Empty(actual[0].Children);

            Assert.Equal("Create", actual[0].Action);
            Assert.Equal("Roles", actual[0].Controller);
            Assert.Equal("Administration", actual[0].Area);
            Assert.Equal("far fa-file", actual[0].IconClass);
        }

        [Fact]
        public void For_ReturnsAuthorizedNodes()
        {
            authorization.IsGrantedFor(context.HttpContext.User.Id(), "Administration", "Accounts", "Index").Returns(true);

            SiteMapNode[] actual = siteMap.For(context).ToArray();

            Assert.Single(actual);

            Assert.Null(actual[0].Action);
            Assert.Null(actual[0].Controller);
            Assert.Equal("Administration", actual[0].Area);
            Assert.Equal("fa fa-cogs", actual[0].IconClass);

            actual = actual[0].Children.ToArray();

            Assert.Single(actual);

            Assert.Empty(actual[0].Children);

            Assert.Equal("Index", actual[0].Action);
            Assert.Equal("Accounts", actual[0].Controller);
            Assert.Equal("Administration", actual[0].Area);
            Assert.Equal("fa fa-user", actual[0].IconClass);
        }

        [Fact]
        public void For_SetsActiveMenu()
        {
            route["action"] = "Create";
            route["controller"] = "Roles";
            route["area"] = "Administration";

            siteMap = new SiteMap(CreateSiteMap(), null);

            SiteMapNode[] actual = siteMap.For(context).ToArray();

            Assert.Single(actual);
            Assert.False(actual[0].IsActive);

            actual = actual[0].Children.ToArray();

            Assert.Equal(2, actual.Length);
            Assert.False(actual[0].IsActive);
            Assert.False(actual[1].IsActive);
            Assert.Empty(actual[0].Children);

            actual = actual[1].Children.ToArray();

            Assert.Empty(actual[0].Children);
            Assert.True(actual[0].IsActive);
            Assert.Single(actual);
        }

        [Fact]
        public void For_NonMenuChildrenNodeIsActive_SetsActiveMenu()
        {
            route["action"] = "Edit";
            route["controller"] = "Accounts";
            route["area"] = "Administration";

            siteMap = new SiteMap(CreateSiteMap(), null);

            SiteMapNode[] actual = siteMap.For(context).ToArray();

            Assert.Single(actual);
            Assert.False(actual[0].IsActive);

            actual = actual[0].Children.ToArray();

            Assert.Equal(2, actual.Length);
            Assert.True(actual[0].IsActive);
            Assert.False(actual[1].IsActive);
            Assert.Empty(actual[0].Children);

            actual = actual[1].Children.ToArray();

            Assert.Single(actual);
            Assert.False(actual[0].IsActive);
            Assert.Empty(actual[0].Children);
        }

        [Fact]
        public void For_ActiveMenuParents_SetsHasActiveChildren()
        {
            route["action"] = "Create";
            route["controller"] = "Roles";
            route["area"] = "Administration";

            siteMap = new SiteMap(CreateSiteMap(), null);

            SiteMapNode[] actual = siteMap.For(context).ToArray();

            Assert.Single(actual);
            Assert.True(actual[0].HasActiveChildren);

            actual = actual[0].Children.ToArray();

            Assert.Equal(2, actual.Length);
            Assert.Empty(actual[0].Children);
            Assert.True(actual[1].HasActiveChildren);
            Assert.False(actual[0].HasActiveChildren);

            actual = actual[1].Children.ToArray();

            Assert.Single(actual);
            Assert.Empty(actual[0].Children);
            Assert.False(actual[0].HasActiveChildren);
        }

        [Fact]
        public void For_RemovesEmptyNodes()
        {
            authorization.IsGrantedFor(Arg.Any<Int32?>(), Arg.Any<String>(), Arg.Any<String>(), Arg.Any<String>()).Returns(true);
            authorization.IsGrantedFor(context.HttpContext.User.Id(), "Administration", "Roles", "Create").Returns(false);

            SiteMapNode[] actual = siteMap.For(context).ToArray();

            Assert.Single(actual);

            Assert.Null(actual[0].Action);
            Assert.Null(actual[0].Controller);
            Assert.Equal("Administration", actual[0].Area);
            Assert.Equal("fa fa-cogs", actual[0].IconClass);

            actual = actual[0].Children.ToArray();

            Assert.Single(actual);

            Assert.Empty(actual[0].Children);

            Assert.Equal("Index", actual[0].Action);
            Assert.Equal("Accounts", actual[0].Controller);
            Assert.Equal("Administration", actual[0].Area);
            Assert.Equal("fa fa-user", actual[0].IconClass);
        }

        #endregion

        #region BreadcrumbFor(ViewContext context)

        [Fact]
        public void BreadcrumbFor_IsCaseInsensitive()
        {
            route["controller"] = "profile";
            route["action"] = "edit";
            route["area"] = null;

            SiteMapNode[] actual = siteMap.BreadcrumbFor(context).ToArray();

            Assert.Equal(3, actual.Length);

            Assert.Equal("fa fa-home", actual[0].IconClass);
            Assert.Equal("Home", actual[0].Controller);
            Assert.Equal("Index", actual[0].Action);
            Assert.Null(actual[0].Area);

            Assert.Equal("fa fa-user", actual[1].IconClass);
            Assert.Equal("Profile", actual[1].Controller);
            Assert.Null(actual[1].Action);
            Assert.Null(actual[1].Area);

            Assert.Equal("fa fa-pencil-alt", actual[2].IconClass);
            Assert.Equal("Profile", actual[2].Controller);
            Assert.Equal("Edit", actual[2].Action);
            Assert.Null(actual[2].Area);
        }

        [Fact]
        public void BreadcrumbFor_NoAction_ReturnsEmpty()
        {
            route["controller"] = "profile";
            route["action"] = "edit";
            route["area"] = "area";

            Assert.Empty(siteMap.BreadcrumbFor(context));
        }

        #endregion

        #region Test helpers

        private static String CreateSiteMap()
        {
            return @"<siteMap>
                <siteMapNode icon=""fa fa-home"" controller=""Home"" action=""Index"">
                    <siteMapNode icon=""fa fa-user"" controller=""Profile"">
                        <siteMapNode icon=""fa fa-pencil-alt"" controller=""Profile"" action=""Edit"" />
                    </siteMapNode>
                    <siteMapNode menu=""true"" icon=""fa fa-cogs"" area=""Administration"">
                        <siteMapNode menu=""true"" icon=""fa fa-user"" area=""Administration"" controller=""Accounts"" action=""Index"">
                            <siteMapNode icon=""fa fa-info"" area=""Administration"" controller=""Accounts"" action=""Details"">
                                <siteMapNode icon=""fa fa-pencil-alt"" area=""Administration"" controller=""Accounts"" action=""Edit"" />
                            </siteMapNode>
                        </siteMapNode>
                        <siteMapNode menu=""true"" icon=""fa fa-users"" area=""Administration"" controller=""Roles"">
                            <siteMapNode menu=""true"" icon=""far fa-file"" area=""Administration"" controller=""Roles"" action=""Create"" />
                            <siteMapNode icon=""fa fa-pencil-alt"" area=""Administration"" controller=""Roles"" action=""Edit"" />
                        </siteMapNode>
                    </siteMapNode>
                </siteMapNode>
            </siteMap>";
        }

        #endregion
    }
}
