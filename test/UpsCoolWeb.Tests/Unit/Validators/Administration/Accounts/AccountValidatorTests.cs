using UpsCoolWeb.Components.Notifications;
using UpsCoolWeb.Components.Security;
using UpsCoolWeb.Data.Core;
using UpsCoolWeb.Objects;
using UpsCoolWeb.Resources;
using UpsCoolWeb.Tests;
using NSubstitute;
using System;
using System.Linq;
using Xunit;

namespace UpsCoolWeb.Validators.Tests
{
    public class AccountValidatorTests : IDisposable
    {
        private AccountValidator validator;
        private TestingContext context;
        private Account account;
        private IHasher hasher;

        public AccountValidatorTests()
        {
            context = new TestingContext();
            hasher = Substitute.For<IHasher>();
            validator = new AccountValidator(new UnitOfWork(context), hasher);
            hasher.VerifyPassword(Arg.Any<String>(), Arg.Any<String>()).Returns(true);

            context.Add(account = ObjectsFactory.CreateAccount());
            context.SaveChanges();

            validator.CurrentAccountId = account.Id;
        }
        public void Dispose()
        {
            validator.Dispose();
            context.Dispose();
        }

        #region CanRecover(AccountRecoveryView view)

        [Fact]
        public void CanRecover_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanRecover(ObjectsFactory.CreateAccountRecoveryView()));
        }

        [Fact]
        public void CanRecover_ValidAccount()
        {
            Assert.True(validator.CanRecover(ObjectsFactory.CreateAccountRecoveryView()));
            Assert.Empty(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        #endregion

        #region CanReset(AccountResetView view)

        [Fact]
        public void CanReset_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanReset(ObjectsFactory.CreateAccountResetView()));
        }

        [Fact]
        public void CanReset_ExpiredToken_ReturnsFalse()
        {
            account.RecoveryTokenExpirationDate = DateTime.Now.AddMinutes(-5);
            context.Update(account);
            context.SaveChanges();

            Boolean canReset = validator.CanReset(ObjectsFactory.CreateAccountResetView());
            Alert alert = validator.Alerts.Single();

            Assert.False(canReset);
            Assert.Equal(0, alert.Timeout);
            Assert.Empty(validator.ModelState);
            Assert.Equal(AlertType.Danger, alert.Type);
            Assert.Equal(Validation.For<AccountView>("ExpiredToken"), alert.Message);
        }

        [Fact]
        public void CanReset_ValidAccount()
        {
            Assert.True(validator.CanReset(ObjectsFactory.CreateAccountResetView()));
            Assert.Empty(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        #endregion

        #region CanLogin(AccountLoginView view)

        [Fact]
        public void CanLogin_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanLogin(ObjectsFactory.CreateAccountLoginView()));
        }

        [Fact]
        public void CanLogin_NoAccount_ReturnsFalse()
        {
            hasher.VerifyPassword(null, null).Returns(false);
            AccountLoginView view = new AccountLoginView();

            Boolean canLogin = validator.CanLogin(view);
            Alert alert = validator.Alerts.Single();

            Assert.False(canLogin);
            Assert.Equal(0, alert.Timeout);
            Assert.Empty(validator.ModelState);
            Assert.Equal(AlertType.Danger, alert.Type);
            Assert.Equal(Validation.For<AccountView>("IncorrectAuthentication"), alert.Message);
        }

        [Fact]
        public void CanLogin_IncorrectPassword_ReturnsFalse()
        {
            account = context.Set<Account>().Single();
            account.IsLocked = true;
            context.SaveChanges();

            AccountLoginView view = ObjectsFactory.CreateAccountLoginView();
            hasher.VerifyPassword(view.Password, Arg.Any<String>()).Returns(false);

            Boolean canLogin = validator.CanLogin(view);
            Alert alert = validator.Alerts.Single();

            Assert.False(canLogin);
            Assert.Equal(0, alert.Timeout);
            Assert.Empty(validator.ModelState);
            Assert.Equal(AlertType.Danger, alert.Type);
            Assert.Equal(Validation.For<AccountView>("IncorrectAuthentication"), alert.Message);
        }

        [Fact]
        public void CanLogin_LockedAccount_ReturnsFalse()
        {
            AccountLoginView view = ObjectsFactory.CreateAccountLoginView();
            account = context.Set<Account>().Single();
            account.IsLocked = true;
            context.SaveChanges();

            Boolean canLogin = validator.CanLogin(view);
            Alert alert = validator.Alerts.Single();

            Assert.False(canLogin);
            Assert.Equal(0, alert.Timeout);
            Assert.Empty(validator.ModelState);
            Assert.Equal(AlertType.Danger, alert.Type);
            Assert.Equal(Validation.For<AccountView>("LockedAccount"), alert.Message);
        }

        [Fact]
        public void CanLogin_IsCaseInsensitive()
        {
            AccountLoginView view = ObjectsFactory.CreateAccountLoginView();
            view.Username = view.Username.ToUpper();

            Assert.True(validator.CanLogin(view));
        }

        [Fact]
        public void CanLogin_ValidAccount()
        {
            Assert.True(validator.CanLogin(ObjectsFactory.CreateAccountLoginView()));
            Assert.Empty(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        #endregion

        #region CanCreate(AccountCreateView view)

        [Fact]
        public void CanCreate_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanCreate(ObjectsFactory.CreateAccountCreateView(1)));
        }

        [Fact]
        public void CanCreate_UsedUsername_ReturnsFalse()
        {
            AccountCreateView view = ObjectsFactory.CreateAccountCreateView(1);
            view.Username = account.Username.ToLower();

            Boolean canCreate = validator.CanCreate(view);

            Assert.False(canCreate);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validation.For<AccountView>("UniqueUsername"), validator.ModelState["Username"].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanCreate_UsedEmail_ReturnsFalse()
        {
            AccountCreateView view = ObjectsFactory.CreateAccountCreateView(1);
            view.Email = account.Email.ToUpper();

            Boolean canCreate = validator.CanCreate(view);

            Assert.False(canCreate);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validation.For<AccountView>("UniqueEmail"), validator.ModelState["Email"].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanCreate_ValidAccount()
        {
            Assert.True(validator.CanCreate(ObjectsFactory.CreateAccountCreateView(1)));
            Assert.Empty(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        #endregion

        #region CanEdit(AccountEditView view)

        [Fact]
        public void CanEdit_Account_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanEdit(ObjectsFactory.CreateAccountEditView(account.Id)));
        }

        [Fact]
        public void CanEdit_Account_UsedUsername_ReturnsFalse()
        {
            Account usedAccount = ObjectsFactory.CreateAccount(1);
            context.Add(usedAccount);
            context.SaveChanges();

            AccountEditView view = ObjectsFactory.CreateAccountEditView(account.Id);
            view.Username = usedAccount.Username.ToLower();

            Boolean canEdit = validator.CanEdit(view);

            Assert.False(canEdit);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validation.For<AccountView>("UniqueUsername"), validator.ModelState["Username"].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanEdit_Account_ToSameUsername()
        {
            AccountEditView view = ObjectsFactory.CreateAccountEditView(account.Id);
            view.Username = account.Username.ToUpper();

            Assert.True(validator.CanEdit(view));
        }

        [Fact]
        public void CanEdit_Account_UsedEmail_ReturnsFalse()
        {
            Account usedAccount = ObjectsFactory.CreateAccount(1);
            context.Add(usedAccount);
            context.SaveChanges();

            AccountEditView view = ObjectsFactory.CreateAccountEditView(account.Id);
            view.Email = usedAccount.Email;

            Boolean canEdit = validator.CanEdit(view);

            Assert.False(canEdit);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validation.For<AccountView>("UniqueEmail"), validator.ModelState["Email"].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanEdit_Account_ToSameEmail()
        {
            AccountEditView view = ObjectsFactory.CreateAccountEditView(account.Id);
            view.Email = account.Email.ToUpper();

            Assert.True(validator.CanEdit(view));
        }

        [Fact]
        public void CanEdit_ValidAccount()
        {
            Assert.True(validator.CanEdit(ObjectsFactory.CreateAccountEditView(account.Id)));
            Assert.Empty(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        #endregion

        #region CanEdit(ProfileEditView view)

        [Fact]
        public void CanEdit_Profile_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanEdit(ObjectsFactory.CreateProfileEditView()));
        }

        [Fact]
        public void CanEdit_Profile_IncorrectPassword_ReturnsFalse()
        {
            ProfileEditView view = ObjectsFactory.CreateProfileEditView();
            hasher.VerifyPassword(view.Password, Arg.Any<String>()).Returns(false);

            Boolean canEdit = validator.CanEdit(view);

            Assert.False(canEdit);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validation.For<AccountView>("IncorrectPassword"), validator.ModelState["Password"].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanEdit_Profile_UsedUsername_ReturnsFalse()
        {
            Account usedAccount = ObjectsFactory.CreateAccount(1);
            context.Add(usedAccount);
            context.SaveChanges();

            ProfileEditView view = ObjectsFactory.CreateProfileEditView();
            view.Username = usedAccount.Username.ToLower();

            Boolean canEdit = validator.CanEdit(view);

            Assert.False(canEdit);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validation.For<AccountView>("UniqueUsername"), validator.ModelState["Username"].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanEdit_Profile_ToSameUsername()
        {
            ProfileEditView view = ObjectsFactory.CreateProfileEditView();
            view.Username = account.Username.ToUpper();

            Assert.True(validator.CanEdit(view));
        }

        [Fact]
        public void CanEdit_Profile_UsedEmail_ReturnsFalse()
        {
            Account usedAccount = ObjectsFactory.CreateAccount(1);
            context.Add(usedAccount);
            context.SaveChanges();

            ProfileEditView view = ObjectsFactory.CreateProfileEditView();
            view.Email = usedAccount.Email;

            Boolean canEdit = validator.CanEdit(view);

            Assert.False(canEdit);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validation.For<AccountView>("UniqueEmail"), validator.ModelState["Email"].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanEdit_Profile_ToSameEmail()
        {
            ProfileEditView view = ObjectsFactory.CreateProfileEditView();
            view.Email = account.Email.ToUpper();

            Assert.True(validator.CanEdit(view));
        }

        [Fact]
        public void CanEdit_ValidProfile()
        {
            Assert.True(validator.CanEdit(ObjectsFactory.CreateProfileEditView()));
            Assert.Empty(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        #endregion

        #region CanDelete(ProfileDeleteView view)

        [Fact]
        public void CanDelete_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanDelete(ObjectsFactory.CreateProfileDeleteView()));
        }

        [Fact]
        public void CanDelete_IncorrectPassword_ReturnsFalse()
        {
            ProfileDeleteView view = ObjectsFactory.CreateProfileDeleteView();
            hasher.VerifyPassword(view.Password, Arg.Any<String>()).Returns(false);

            Boolean canDelete = validator.CanDelete(view);

            Assert.False(canDelete);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validation.For<AccountView>("IncorrectPassword"), validator.ModelState["Password"].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanDelete_ValidProfile()
        {
            Assert.True(validator.CanDelete(ObjectsFactory.CreateProfileDeleteView()));
            Assert.Empty(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        #endregion
    }
}
