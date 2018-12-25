using UpsCoolWeb.Data.Migrations;
using UpsCoolWeb.Objects;
using UpsCoolWeb.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace UpsCoolWeb.Resources.Tests
{
    public class ResourcesTests
    {
        [Fact]
        public void Resources_HasAllPageTitles()
        {
            IDictionary<String, Object> values = new Dictionary<String, Object>();
            IEnumerable<XElement> sitemap = XDocument
                .Load("../../../../../src/UpsCoolWeb.Web/mvc.sitemap")
                .Descendants("siteMapNode")
                .Where(node => node.Attribute("action") != null);

            foreach (XElement node in sitemap)
            {
                values["area"] = node.Attribute("area")?.Value;
                values["action"] = node.Attribute("action").Value;
                values["controller"] = node.Attribute("controller").Value;

                String page = $"{values["area"]}{values["controller"]}{values["action"]}";

                Assert.True(!String.IsNullOrEmpty(Resource.ForPage(values)),
                    $"'{page}' page, does not have a title.");
            }
        }

        [Fact]
        public void Resources_HasAllSiteMapTitles()
        {
            IEnumerable<XElement> sitemap = XDocument
                .Load("../../../../../src/UpsCoolWeb.Web/mvc.sitemap")
                .Descendants("siteMapNode");

            foreach (XElement node in sitemap)
                Assert.True(!String.IsNullOrEmpty(Resource.ForSiteMap(node.Attribute("area")?.Value, node.Attribute("controller")?.Value, node.Attribute("action")?.Value)),
                    $"Sitemap node '{node}' page, does not have a title.");
        }

        [Fact]
        public void Resources_HasAllPermissionAreaTitles()
        {
            using (TestingContext context = new TestingContext())
            using (Configuration configuration = new Configuration(context, null))
            {
                configuration.SeedData();

                foreach (Permission permission in context.Set<Permission>().Where(permission => permission.Area != null))
                    Assert.True(!String.IsNullOrEmpty(Resource.ForPermission(permission.Area)),
                        $"'{permission.Area}' permission, does not have a title.");
            }
        }

        [Fact]
        public void Resources_HasAllPermissionControllerTitles()
        {
            using (TestingContext context = new TestingContext())
            using (Configuration configuration = new Configuration(context, null))
            {
                configuration.SeedData();

                foreach (Permission permission in context.Set<Permission>())
                    Assert.True(!String.IsNullOrEmpty(Resource.ForPermission(permission.Area, permission.Controller)),
                        $"'{permission.Area}{permission.Controller}' permission, does not have a title.");
            }
        }

        [Fact]
        public void Resources_HasAllPermissionActionTitles()
        {
            using (TestingContext context = new TestingContext())
            using (Configuration configuration = new Configuration(context, null))
            {
                configuration.SeedData();

                foreach (Permission permission in context.Set<Permission>())
                    Assert.True(!String.IsNullOrEmpty(Resource.ForPermission(permission.Area, permission.Controller, permission.Action)),
                        $"'{permission.Area}{permission.Controller}{permission.Action} permission', does not have a title.");
            }
        }
    }
}
