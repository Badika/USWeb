using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UpsCoolWeb.Components.Security;
using UpsCoolWeb.Data.Core;
using UpsCoolWeb.Objects;
using UpsCoolWeb.Tests;
using NSubstitute;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Xunit;

namespace UpsCoolWeb.Services.Tests
{
    public class AccountServiceTests : IDisposable
    {
        private AccountService service;
        private TestingContext context;
        private Account account;
        private IHasher hasher;

        public AccountServiceTests()
        {
            context = new TestingContext();
            hasher = Substitute.For<IHasher>();
            service = new AccountService(new UnitOfWork(context), hasher);
            hasher.HashPassword(Arg.Any<String>()).Returns(info => info.Arg<String>() + "Hashed");

            context.Add(account = ObjectsFactory.CreateAccount());
            context.SaveChanges();

            service.CurrentAccountId = account.Id;
        }
        public void Dispose()
        {
            service.Dispose();
            context.Dispose();
        }

        #region Get<TView>(Int32 id)

        [Fact]
        public void Get_ReturnsViewById()
        {
            AccountView actual = service.Get<AccountView>(account.Id);
            AccountView expected = Mapper.Map<AccountView>(account);

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.RoleTitle, actual.RoleTitle);
            Assert.Equal(expected.IsLocked, actual.IsLocked);
            Assert.Equal(expected.Username, actual.Username);
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.Id, actual.Id);
        }

        #endregion

        #region GetViews()

        [Fact]
        public void GetViews_ReturnsAccountViews()
        {
            AccountView[] actual = service.GetViews().ToArray();
            AccountView[] expected = context
                .Set<Account>()
                .ProjectTo<AccountView>()
                .OrderByDescending(view => view.Id)
                .ToArray();

            for (Int32 i = 0; i < expected.Length || i < actual.Length; i++)
            {
                Assert.Equal(expected[i].CreationDate, actual[i].CreationDate);
                Assert.Equal(expected[i].RoleTitle, actual[i].RoleTitle);
                Assert.Equal(expected[i].IsLocked, actual[i].IsLocked);
                Assert.Equal(expected[i].Username, actual[i].Username);
                Assert.Equal(expected[i].Email, actual[i].Email);
                Assert.Equal(expected[i].Id, actual[i].Id);
            }
        }

        #endregion

        #region IsLoggedIn(IPrincipal user)

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsLoggedIn_ReturnsIsAuthenticated(Boolean isAuthenticated)
        {
            IPrincipal user = Substitute.For<IPrincipal>();
            user.Identity.IsAuthenticated.Returns(isAuthenticated);

            Boolean actual = service.IsLoggedIn(user);
            Boolean expected = isAuthenticated;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region IsActive(Int32 id)

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsActive_ReturnsAccountState(Boolean isLocked)
        {
            account.IsLocked = isLocked;
            context.Update(account);
            context.SaveChanges();

            Boolean actual = service.IsActive(account.Id);
            Boolean expected = !isLocked;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsActive_NoAccount_ReturnsFalse()
        {
            Assert.False(service.IsActive(0));
        }

        #endregion

        #region Recover(AccountRecoveryView view)

        [Fact]
        public void Recover_NoEmail_ReturnsNull()
        {
            AccountRecoveryView view = ObjectsFactory.CreateAccountRecoveryView();
            view.Email = "not@existing.email";

            Assert.Null(service.Recover(view));
        }

        [Fact]
        public void Recover_Information()
        {
            AccountRecoveryView view = ObjectsFactory.CreateAccountRecoveryView();
            account.RecoveryTokenExpirationDate = DateTime.Now.AddMinutes(30);
            String oldToken = account.RecoveryToken;
            view.Email = view.Email.ToUpper();

            account.RecoveryToken = service.Recover(view);

            Account actual = context.Set<Account>().AsNoTracking().Single();
            Account expected = account;

            Assert.InRange(actual.RecoveryTokenExpirationDate.Value.Ticks,
                expected.RecoveryTokenExpirationDate.Value.Ticks - TimeSpan.TicksPerSecond,
                expected.RecoveryTokenExpirationDate.Value.Ticks + TimeSpan.TicksPerSecond);
            Assert.Equal(expected.RecoveryToken, actual.RecoveryToken);
            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.IsLocked, actual.IsLocked);
            Assert.Equal(expected.Passhash, actual.Passhash);
            Assert.Equal(expected.Username, actual.Username);
            Assert.NotEqual(oldToken, actual.RecoveryToken);
            Assert.Equal(expected.RoleId, actual.RoleId);
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.Id, actual.Id);
            Assert.NotNull(actual.RecoveryToken);
        }

        #endregion

        #region Reset(AccountResetView view)

        [Fact]
        public void Reset_Account()
        {
            AccountResetView view = ObjectsFactory.CreateAccountResetView();

            service.Reset(view);

            Account actual = context.Set<Account>().AsNoTracking().Single();
            Account expected = account;

            Assert.Equal(hasher.HashPassword(view.NewPassword), actual.Passhash);
            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.IsLocked, actual.IsLocked);
            Assert.Equal(expected.Username, actual.Username);
            Assert.Null(actual.RecoveryTokenExpirationDate);
            Assert.Equal(expected.RoleId, actual.RoleId);
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Null(actual.RecoveryToken);
        }

        #endregion

        #region Create(AccountCreateView view)

        [Fact]
        public void Create_Account()
        {
            AccountCreateView view = ObjectsFactory.CreateAccountCreateView(1);
            view.Email = view.Email.ToUpper();
            view.RoleId = account.RoleId;

            service.Create(view);

            Account actual = context.Set<Account>().AsNoTracking().Single(model => model.Id != account.Id);
            AccountCreateView expected = view;

            Assert.Equal(hasher.HashPassword(expected.Password), actual.Passhash);
            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Email.ToLower(), actual.Email);
            Assert.Equal(expected.Username, actual.Username);
            Assert.Null(actual.RecoveryTokenExpirationDate);
            Assert.Equal(expected.RoleId, actual.RoleId);
            Assert.Null(actual.RecoveryToken);
            Assert.False(actual.IsLocked);
        }

        #endregion

        #region Edit(AccountEditView view)

        [Fact]
        public void Edit_Account()
        {
            AccountEditView view = ObjectsFactory.CreateAccountEditView(account.Id);
            view.IsLocked = account.IsLocked = !account.IsLocked;
            view.Email = (account.Email += "s").ToUpper();
            view.Username = account.Username += "Test";
            view.RoleId = account.RoleId = null;

            service.Edit(view);

            Account actual = context.Set<Account>().AsNoTracking().Single();
            Account expected = account;

            Assert.Equal(expected.RecoveryTokenExpirationDate, actual.RecoveryTokenExpirationDate);
            Assert.Equal(expected.RecoveryToken, actual.RecoveryToken);
            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.IsLocked, actual.IsLocked);
            Assert.Equal(expected.Username, actual.Username);
            Assert.Equal(expected.Passhash, actual.Passhash);
            Assert.Equal(expected.RoleId, actual.RoleId);
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.Id, actual.Id);
        }

        #endregion

        #region Edit(ProfileEditView view)

        [Fact]
        public void Edit_Profile()
        {
            ProfileEditView view = ObjectsFactory.CreateProfileEditView();
            account.Passhash = hasher.HashPassword(view.NewPassword);
            view.Username = account.Username += "Test";
            view.Email = account.Email += "Test";

            service.Edit(view);

            Account actual = context.Set<Account>().AsNoTracking().Single();
            Account expected = account;

            Assert.Equal(expected.RecoveryTokenExpirationDate, actual.RecoveryTokenExpirationDate);
            Assert.Equal(expected.RecoveryToken, actual.RecoveryToken);
            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Email.ToLower(), actual.Email);
            Assert.Equal(expected.IsLocked, actual.IsLocked);
            Assert.Equal(expected.Username, actual.Username);
            Assert.Equal(expected.Passhash, actual.Passhash);
            Assert.Equal(expected.RoleId, actual.RoleId);
            Assert.Equal(expected.Id, actual.Id);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Edit_NullOrEmptyNewPassword_DoesNotEditPassword(String newPassword)
        {
            ProfileEditView view = ObjectsFactory.CreateProfileEditView();
            view.NewPassword = newPassword;

            service.Edit(view);

            String actual = context.Set<Account>().AsNoTracking().Single().Passhash;
            String expected = account.Passhash;

            Assert.Equal(expected, actual);
        }

        #endregion

        #region Delete(Int32 id)

        [Fact]
        public void Delete_Account()
        {
            service.Delete(account.Id);

            Assert.Empty(context.Set<Account>());
        }

        #endregion

        #region Login(HttpContext context, String username)

        [Fact]
        public void Login_Account()
        {
            HttpContext httpContext = Substitute.For<HttpContext>();
            httpContext.RequestServices.GetService(typeof(IAuthenticationService)).Returns(Substitute.For<IAuthenticationService>());

            service.Login(httpContext, account.Username.ToUpper());

            httpContext.Received().SignInAsync("Cookies", Arg.Is<ClaimsPrincipal>(principal =>
                principal.Claims.Single().Subject.Name == account.Id.ToString() &&
                principal.Claims.Single().Subject.NameClaimType == "name" &&
                principal.Claims.Single().Subject.RoleClaimType == "role" &&
                principal.Identity.AuthenticationType == "local" &&
                principal.Identity.Name == account.Id.ToString() &&
                principal.Identity.IsAuthenticated));
        }

        #endregion

        #region Logout(HttpContext context)

        [Fact]
        public void Logout_Account()
        {
            HttpContext httpContext = Substitute.For<HttpContext>();
            httpContext.RequestServices.GetService(typeof(IAuthenticationService)).Returns(Substitute.For<IAuthenticationService>());

            service.Logout(httpContext);

            httpContext.Received().SignOutAsync("Cookies");
        }

        #endregion
    }
}

