using Microsoft.AspNetCore.Mvc.ModelBinding;
using UpsCoolWeb.Components.Security;
using UpsCoolWeb.Data.Core;
using UpsCoolWeb.Objects;
using UpsCoolWeb.Resources;
using System;
using System.Linq;

namespace UpsCoolWeb.Validators
{
    public class AccountValidator : BaseValidator, IAccountValidator
    {
        private IHasher Hasher { get; }

        public AccountValidator(IUnitOfWork unitOfWork, IHasher hasher)
            : base(unitOfWork)
        {
            Hasher = hasher;
        }

        public Boolean CanRecover(AccountRecoveryView view)
        {
            return ModelState.IsValid;
        }
        public Boolean CanReset(AccountResetView view)
        {
            Boolean isValid = IsValidResetToken(view.Token);
            isValid &= ModelState.IsValid;

            return isValid;
        }
        public Boolean CanLogin(AccountLoginView view)
        {
            Boolean isValid = IsAuthenticated(view.Username, view.Password);
            isValid = isValid && IsActive(view.Username);
            isValid &= ModelState.IsValid;

            return isValid;
        }

        public Boolean CanCreate(AccountCreateView view)
        {
            Boolean isValid = IsUniqueUsername(view.Id, view.Username);
            isValid &= IsUniqueEmail(view.Id, view.Email);
            isValid &= ModelState.IsValid;

            return isValid;
        }
        public Boolean CanEdit(AccountEditView view)
        {
            Boolean isValid = IsUniqueUsername(view.Id, view.Username);
            isValid &= IsUniqueEmail(view.Id, view.Email);
            isValid &= ModelState.IsValid;

            return isValid;
        }

        public Boolean CanEdit(ProfileEditView view)
        {
            Boolean isValid = IsUniqueUsername(CurrentAccountId, view.Username);
            isValid &= IsCorrectPassword(CurrentAccountId, view.Password);
            isValid &= IsUniqueEmail(CurrentAccountId, view.Email);
            isValid &= ModelState.IsValid;

            return isValid;
        }
        public Boolean CanDelete(ProfileDeleteView view)
        {
            Boolean isValid = IsCorrectPassword(CurrentAccountId, view.Password);
            isValid &= ModelState.IsValid;

            return isValid;
        }

        private Boolean IsUniqueUsername(Int32 accountId, String username)
        {
            Boolean isUnique = !UnitOfWork
                .Select<Account>()
                .Any(account =>
                    account.Id != accountId &&
                    account.Username.ToLower() == (username ?? "").ToLower());

            if (!isUnique)
                ModelState.AddModelError<AccountView>(model => model.Username,
                    Validation.For<AccountView>("UniqueUsername"));

            return isUnique;
        }
        private Boolean IsUniqueEmail(Int32 accountId, String email)
        {
            Boolean isUnique = !UnitOfWork
                .Select<Account>()
                .Any(account =>
                    account.Id != accountId &&
                    account.Email.ToLower() == (email ?? "").ToLower());

            if (!isUnique)
                ModelState.AddModelError<AccountView>(account => account.Email,
                    Validation.For<AccountView>("UniqueEmail"));

            return isUnique;
        }

        private Boolean IsAuthenticated(String username, String password)
        {
            String passhash = UnitOfWork
                .Select<Account>()
                .Where(account => account.Username.ToLower() == (username ?? "").ToLower())
                .Select(account => account.Passhash)
                .SingleOrDefault();

            Boolean isCorrect = Hasher.VerifyPassword(password, passhash);
            if (!isCorrect)
                Alerts.AddError(Validation.For<AccountView>("IncorrectAuthentication"));

            return isCorrect;
        }
        private Boolean IsCorrectPassword(Int32 accountId, String password)
        {
            String passhash = UnitOfWork
                .Select<Account>()
                .Where(account => account.Id == accountId)
                .Select(account => account.Passhash)
                .Single();

            Boolean isCorrect = Hasher.VerifyPassword(password, passhash);
            if (!isCorrect)
                ModelState.AddModelError<ProfileEditView>(account => account.Password,
                    Validation.For<AccountView>("IncorrectPassword"));

            return isCorrect;
        }

        private Boolean IsValidResetToken(String token)
        {
            Boolean isValid = UnitOfWork
                .Select<Account>()
                .Any(account =>
                    account.RecoveryToken == token &&
                    account.RecoveryTokenExpirationDate > DateTime.Now);

            if (!isValid)
                Alerts.AddError(Validation.For<AccountView>("ExpiredToken"));

            return isValid;
        }
        private Boolean IsActive(String username)
        {
            Boolean isActive = UnitOfWork
                .Select<Account>()
                .Any(account =>
                    !account.IsLocked &&
                    account.Username.ToLower() == (username ?? "").ToLower());

            if (!isActive)
                Alerts.AddError(Validation.For<AccountView>("LockedAccount"));

            return isActive;
        }
    }
}
