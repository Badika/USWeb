using UpsCoolWeb.Data.Core;
using UpsCoolWeb.Objects;
using UpsCoolWeb.Tests;
using NSubstitute;
using System;
using System.Reflection;
using Xunit;

namespace UpsCoolWeb.Components.Security.Tests
{
    public class AuthorizationTests
    {
        private TestingContext context;
        private Authorization authorization;

        public AuthorizationTests()
        {
            context = new TestingContext();
            IServiceProvider services = Substitute.For<IServiceProvider>();
            services.GetService(typeof(IUnitOfWork)).Returns(info => new UnitOfWork(new TestingContext(context.DatabaseName)));

            authorization = new Authorization(Assembly.GetExecutingAssembly(), services);
        }

        #region IsGrantedFor(Int32? accountId, String area, String controller, String action)

        [Fact]
        public void IsGrantedFor_AuthorizesControllerByIgnoringCase()
        {
            Int32 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "Action");

            Assert.True(authorization.IsGrantedFor(accountId, "Area", "AUTHORIZED", "Action"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeControllerByIgnoringCase()
        {
            Int32 accountId = CreateAccountWithPermissionFor("Test", "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "AUTHORIZED", "Action"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void IsGrantedFor_AuthorizesControllerWithoutArea(String area)
        {
            Int32 accountId = CreateAccountWithPermissionFor(null, "Authorized", "Action");

            Assert.True(authorization.IsGrantedFor(accountId, area, "Authorized", "Action"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void IsGrantedFor_DoesNotAuthorizeControllerWithoutArea(String area)
        {
            Int32 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, area, "Authorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesControllerWithArea()
        {
            Int32 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "Action");

            Assert.True(authorization.IsGrantedFor(accountId, "Area", "Authorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeControllerWithArea()
        {
            Int32 accountId = CreateAccountWithPermissionFor("Test", "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "Authorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesGetAction()
        {
            Int32 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "AuthorizedGetAction");

            Assert.True(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedGetAction"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeGetAction()
        {
            Int32 accountId = CreateAccountWithPermissionFor("Test", "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedGetAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesNamedGetAction()
        {
            Int32 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "AuthorizedNamedGetAction");

            Assert.True(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedNamedGetAction"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeNamedGetAction()
        {
            Int32 accountId = CreateAccountWithPermissionFor("Test", "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedNamedGetAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesNotExistingAction()
        {
            Assert.True(authorization.IsGrantedFor(null, null, "Authorized", "Test"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesNonGetAction()
        {
            Int32 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "AuthorizedPostAction");

            Assert.True(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedPostAction"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeNonGetAction()
        {
            Int32 accountId = CreateAccountWithPermissionFor("Test", "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedPostAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesNamedNonGetAction()
        {
            Int32 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "AuthorizedNamedPostAction");

            Assert.True(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedNamedPostAction"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeNamedNonGetAction()
        {
            Int32 accountId = CreateAccountWithPermissionFor("Area", "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedNamedPostAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesActionAsAction()
        {
            Int32 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "Action");

            Assert.True(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedAsAction"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeActionAsAction()
        {
            Int32 accountId = CreateAccountWithPermissionFor(null, "Authorized", "Action");

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedAsAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesActionAsSelf()
        {
            Int32 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "AuthorizedAsSelf");

            Assert.True(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedAsSelf"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeActionAsSelf()
        {
            Int32 accountId = CreateAccountWithPermissionFor("Area", "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedAsSelf"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesActionAsOtherAction()
        {
            Int32 accountId = CreateAccountWithPermissionFor(null, "InheritedAuthorized", "InheritanceAction");

            Assert.True(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedAsOtherAction"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeActionAsOtherAction()
        {
            Int32 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "AuthorizedAsOtherAction");

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedAsOtherAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesEmptyAreaAsNull()
        {
            Int32 accountId = CreateAccountWithPermissionFor(null, "Authorized", "Action");

            Assert.True(authorization.IsGrantedFor(accountId, "", "Authorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeEmptyAreaAsNull()
        {
            Int32 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, "", "Authorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesAuthorizedAction()
        {
            Int32 accountId = CreateAccountWithPermissionFor(null, "AllowAnonymous", "AuthorizedAction");

            Assert.True(authorization.IsGrantedFor(accountId, null, "AllowAnonymous", "AuthorizedAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesAllowAnonymousAction()
        {
            Int32 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.True(authorization.IsGrantedFor(accountId, null, "Authorized", "AllowAnonymousAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesAllowUnauthorizedAction()
        {
            Int32 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.True(authorization.IsGrantedFor(accountId, null, "Authorized", "AllowUnauthorizedAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesAuthorizedController()
        {
            Int32 accountId = CreateAccountWithPermissionFor(null, "Authorized", "Action");

            Assert.True(authorization.IsGrantedFor(accountId, null, "Authorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeAuthorizedController()
        {
            Int32 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, null, "Authorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesAllowAnonymousController()
        {
            Int32 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.True(authorization.IsGrantedFor(accountId, null, "AllowAnonymous", "Action"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesAllowUnauthorizedController()
        {
            Int32 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.True(authorization.IsGrantedFor(accountId, null, "AllowUnauthorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesInheritedAuthorizedController()
        {
            Int32 accountId = CreateAccountWithPermissionFor(null, "InheritedAuthorized", "InheritanceAction");

            Assert.True(authorization.IsGrantedFor(accountId, null, "InheritedAuthorized", "InheritanceAction"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeInheritedAuthorizedController()
        {
            Int32 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, null, "InheritedAuthorized", "InheritanceAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesInheritedAllowAnonymousController()
        {
            Int32 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.True(authorization.IsGrantedFor(accountId, null, "InheritedAllowAnonymous", "InheritanceAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesInheritedAllowUnauthorizedController()
        {
            Int32 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.True(authorization.IsGrantedFor(accountId, null, "InheritedAllowUnauthorized", "InheritanceAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesNotAttributedController()
        {
            Int32 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.True(authorization.IsGrantedFor(accountId, null, "NotAttributed", "Action"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeNotExistingAccount()
        {
            CreateAccountWithPermissionFor("Area", "Authorized", "Action");

            Assert.False(authorization.IsGrantedFor(0, "Area", "Authorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeLockedAccount()
        {
            Int32 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "Action", isLocked: true);

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "Authorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeNullAccount()
        {
            CreateAccountWithPermissionFor(null, "Authorized", "Action");

            Assert.False(authorization.IsGrantedFor(null, null, "Authorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesByIgnoringCase()
        {
            Int32 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "Action");

            Assert.True(authorization.IsGrantedFor(accountId, "area", "authorized", "action"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeByIgnoringCase()
        {
            Int32 accountId = CreateAccountWithPermissionFor("Test", "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, "area", "authorized", "action"));
        }

        [Fact]
        public void IsGrantedFor_CachesAccountPermissions()
        {
            Int32 accountId = CreateAccountWithPermissionFor(null, "Authorized", "Action");

            context.Database.EnsureDeleted();

            Assert.True(authorization.IsGrantedFor(accountId, null, "Authorized", "Action"));
        }

        #endregion

        #region Refresh()

        [Fact]
        public void Refresh_Permissions()
        {
            Int32 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "Action");
            Assert.True(authorization.IsGrantedFor(accountId, "Area", "Authorized", "Action"));

            context.Database.EnsureDeleted();

            authorization.Refresh();

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "Authorized", "Action"));
        }

        #endregion

        #region Test helpers

        private Int32 CreateAccountWithPermissionFor(String area, String controller, String action, Boolean isLocked = false)
        {
            using (TestingContext testingContext = new TestingContext(context.DatabaseName))
            {
                RolePermission rolePermission = ObjectsFactory.CreateRolePermission();
                Account account = ObjectsFactory.CreateAccount();
                account.Role.Permissions.Add(rolePermission);
                rolePermission.Role = account.Role;
                account.IsLocked = isLocked;

                rolePermission.Permission.Controller = controller;
                rolePermission.Permission.Action = action;
                rolePermission.Permission.Area = area;

                testingContext.Add(account);
                testingContext.SaveChanges();

                authorization.Refresh();

                return account.Id;
            }
        }

        #endregion
    }
}
