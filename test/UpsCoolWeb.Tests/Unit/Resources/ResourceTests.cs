using UpsCoolWeb.Objects;
using UpsCoolWeb.Tests;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace UpsCoolWeb.Resources.Tests
{
    public class ResourceTests
    {
        #region Set(String type)

        [Fact]
        public void Set_Same()
        {
            Object expected = Resource.Set("Test");
            Object actual = Resource.Set("Test");

            Assert.Same(expected, actual);
        }

        #endregion

        #region ForAction(String name)

        [Fact]
        public void ForAction_IsCaseInsensitive()
        {
            String actual = Resource.ForAction("create");
            String expected = "Create";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForAction_NotFound_ReturnsNull()
        {
            Assert.Null(Resource.ForAction("Null"));
        }

        #endregion

        #region ForLookup(String type)

        [Fact]
        public void ForLookup_IsCaseInsensitive()
        {
            String actual = Resource.ForLookup("role");
            String expected = "Roles";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForLookup_NotFound_ReturnsNull()
        {
            Assert.Null(Resource.ForLookup("Test"));
        }

        #endregion

        #region ForString(String value)

        [Fact]
        public void ForString_IsCaseInsensitive()
        {
            String actual = Resource.ForString("all");
            String expected = "All";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForString_NotFound_ReturnsNull()
        {
            Assert.Null(Resource.ForString("Null"));
        }

        #endregion

        #region ForPage(String header)

        [Fact]
        public void ForPage_Header_IsCaseInsensitive()
        {
            String actual = Resource.ForPage("account");
            String expected = "Account";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForPage_NotFoundHeader_ReturnsNull()
        {
            Assert.Null(Resource.ForPage("Test"));
        }

        #endregion

        #region ForPage(IDictionary<String, Object> values)

        [Fact]
        public void ForPage_IsCaseInsensitive()
        {
            IDictionary<String, Object> values = new Dictionary<String, Object>();
            values["area"] = "administration";
            values["controller"] = "roles";
            values["action"] = "details";

            String actual = Resource.ForPage(values);
            String expected = "Role details";

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ForPage_WithoutArea(String area)
        {
            IDictionary<String, Object> values = new Dictionary<String, Object>();
            values["controller"] = "profile";
            values["action"] = "edit";
            values["area"] = area;

            String actual = Resource.ForPage(values);
            String expected = "Profile edit";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForPage_NotFound_ReturnsNull()
        {
            IDictionary<String, Object> values = new Dictionary<String, Object>
            {
                ["controller"] = null,
                ["action"] = null,
                ["area"] = null
            };

            Assert.Null(Resource.ForPage(values));
        }

        #endregion

        #region ForSiteMap(String area, String controller, String action)

        [Fact]
        public void ForSiteMap_IsCaseInsensitive()
        {
            String actual = Resource.ForSiteMap("administration", "roles", "index");
            String expected = "Roles";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForSiteMap_WithoutControllerAndAction()
        {
            String actual = Resource.ForSiteMap("administration", null, null);
            String expected = "Administration";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForSiteMap_NotFound_ReturnsNull()
        {
            Assert.Null(Resource.ForSiteMap("Test", "Test", "Test"));
        }

        #endregion

        #region ForPermission(String area)

        [Fact]
        public void ForPermission_IsCaseInsensitive()
        {
            String actual = Resource.ForPermission("administration");
            String expected = "Administration";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForPermission_NotFound_ReturnsNull()
        {
            Assert.Null(Resource.ForPermission("Test"));
        }

        [Fact]
        public void ForPermission_NullArea_ReturnsNull()
        {
            Assert.Null(Resource.ForPermission(null));
        }

        #endregion

        #region ForPermission(String area, String controller)

        [Fact]
        public void ForPermission_ReturnsControllerTitle()
        {
            String actual = Resource.ForPermission("Administration", "Roles");
            String expected = "Roles";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForPermission_NotFoundController_ReturnsNull()
        {
            Assert.Null(Resource.ForPermission("", ""));
        }

        #endregion

        #region ForPermission(String area, String controller, String action)

        [Fact]
        public void ForPermission_ReturnsActionTitle()
        {
            String actual = Resource.ForPermission("administration", "accounts", "index");
            String expected = "Index";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForPermission_NotFoundAction_ReturnsNull()
        {
            Assert.Null(Resource.ForPermission("", "", ""));
        }

        #endregion

        #region ForProperty<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression)

        [Fact]
        public void ForProperty_NotMemberLambdaExpression_ReturnNull()
        {
            Assert.Null(Resource.ForProperty<TestView, String>(view => view.ToString()));
        }

        [Fact]
        public void ForProperty_FromLambdaExpression()
        {
            String actual = Resource.ForProperty<AccountView, String>(account => account.Username);
            String expected = "Username";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForProperty_FromLambdaExpressionRelation()
        {
            String actual = Resource.ForProperty<AccountEditView, Int32?>(account => account.RoleId);
            String expected = "Role";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForProperty_NotFoundLambdaExpression_ReturnsNull()
        {
            Assert.Null(Resource.ForProperty<AccountView, Int32>(account => account.Id));
        }

        [Fact]
        public void ForProperty_NotFoundLambdaType_ReturnsNull()
        {
            Assert.Null(Resource.ForProperty<TestView, String>(test => test.Title));
        }

        #endregion

        #region ForProperty(String view, String name)

        [Fact]
        public void ForProperty_View()
        {
            String actual = Resource.ForProperty(nameof(AccountView), nameof(AccountView.Username));
            String expected = "Username";

            Assert.Equal(expected, actual);
        }

        #endregion

        #region ForProperty(Type view, String name)

        [Fact]
        public void ForProperty_IsCaseInsensitive()
        {
            String actual = Resource.ForProperty(typeof(AccountView), nameof(AccountView.Username).ToLower());
            String expected = "Username";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForProperty_FromRelation()
        {
            String actual = Resource.ForProperty(typeof(Object), nameof(Account) + nameof(Account.Username));
            String expected = "Username";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForProperty_FromMultipleRelations()
        {
            String actual = Resource.ForProperty(typeof(RoleView), nameof(Account) + nameof(Role) + nameof(Account) + nameof(Account.Username));
            String expected = "Username";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForProperty_NotFoundProperty_ReturnsNull()
        {
            Assert.Null(Resource.ForProperty(typeof(AccountView), "Id"));
        }

        [Fact]
        public void ForProperty_NotFoundTypeProperty_ReturnsNull()
        {
            Assert.Null(Resource.ForProperty(typeof(TestView), "Title"));
        }

        [Fact]
        public void ForProperty_NullKey_ReturnsNull()
        {
            Assert.Null(Resource.ForProperty(typeof(RoleView), null));
        }

        #endregion

        #region ForProperty(Expression expression)

        [Fact]
        public void ForProperty_NotMemberExpression_ReturnNull()
        {
            Expression<Func<TestView, String>> lambda = (view) => view.ToString();

            Assert.Null(Resource.ForProperty(lambda.Body));
        }

        [Fact]
        public void ForProperty_FromExpression()
        {
            Expression<Func<AccountView, String>> lambda = (account) => account.Username;

            String actual = Resource.ForProperty(lambda.Body);
            String expected = "Username";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForProperty_FromExpressionRelation()
        {
            Expression<Func<AccountEditView, Int32?>> lambda = (account) => account.RoleId;

            String actual = Resource.ForProperty(lambda.Body);
            String expected = "Role";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForProperty_NotFoundExpression_ReturnsNull()
        {
            Expression<Func<AccountView, Int32>> lambda = (account) => account.Id;

            Assert.Null(Resource.ForProperty(lambda.Body));
        }

        [Fact]
        public void ForProperty_NotFoundType_ReturnsNull()
        {
            Expression<Func<TestView, String>> lambda = (test) => test.Title;

            Assert.Null(Resource.ForProperty(lambda.Body));
        }

        #endregion
    }
}
