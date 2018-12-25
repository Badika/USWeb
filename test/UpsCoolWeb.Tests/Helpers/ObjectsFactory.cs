using UpsCoolWeb.Objects;
using System;
using System.Collections.Generic;

namespace UpsCoolWeb.Tests
{
    public static class ObjectsFactory
    {
        #region Administration

        public static Account CreateAccount(Int32 id = 0)
        {
            return new Account
            {
                Username = "Username" + id,
                Passhash = "Passhash" + id,

                Email = id + "@tests.com",

                IsLocked = false,

                RecoveryToken = "Token" + id,
                RecoveryTokenExpirationDate = DateTime.Now.AddMinutes(5),

                Role = CreateRole(id)
            };
        }
        public static AccountView CreateAccountView(Int32 id = 0)
        {
            return new AccountView
            {
                Id = id,

                Username = "Username" + id,
                Email = id + "@tests.com",

                IsLocked = true,

                RoleTitle = "Title" + id
            };
        }
        public static AccountEditView CreateAccountEditView(Int32 id = 0)
        {
            return new AccountEditView
            {
                Id = id,

                Username = "Username" + id,
                Email = id + "@tests.com",

                IsLocked = true,

                RoleId = id
            };
        }
        public static AccountCreateView CreateAccountCreateView(Int32 id = 0)
        {
            return new AccountCreateView
            {
                Username = "Username" + id,
                Password = "Password" + id,

                Email = id + "@tests.com",

                RoleId = id
            };
        }

        public static AccountLoginView CreateAccountLoginView(Int32 id = 0)
        {
            return new AccountLoginView
            {
                Username = "Username" + id,
                Password = "Password" + id
            };
        }
        public static AccountResetView CreateAccountResetView(Int32 id = 0)
        {
            return new AccountResetView
            {
                Token = "Token" + id,
                NewPassword = "NewPassword" + id
            };
        }
        public static AccountRecoveryView CreateAccountRecoveryView(Int32 id = 0)
        {
            return new AccountRecoveryView
            {
                Email = id + "@tests.com"
            };
        }

        public static ProfileEditView CreateProfileEditView(Int32 id = 0)
        {
            return new ProfileEditView
            {
                Id = id,

                Email = id + "@tests.com",
                Username = "Username" + id,

                Password = "Password" + id,
                NewPassword = "NewPassword" + id

            };
        }
        public static ProfileDeleteView CreateProfileDeleteView(Int32 id = 0)
        {
            return new ProfileDeleteView
            {
                Id = id,

                Password = "Password" + id
            };
        }

        public static Role CreateRole(Int32 id = 0)
        {
            return new Role
            {
                Title = "Title" + id,

                Accounts = new List<Account>(),
                Permissions = new List<RolePermission>()
            };
        }
        public static RoleView CreateRoleView(Int32 id = 0)
        {
            return new RoleView
            {
                Id = id,

                Title = "Title" + id
            };
        }

        public static Permission CreatePermission(Int32 id = 0)
        {
            return new Permission
            {
                Id = id,

                Area = "Area" + id,
                Action = "Action" + id,
                Controller = "Controller" + id
            };
        }
        public static RolePermission CreateRolePermission(Int32 id = 0)
        {
            return new RolePermission
            {
                RoleId = id,
                Role = CreateRole(id),

                PermissionId = id,
                Permission = CreatePermission(id)
            };
        }

        #endregion

        #region Tests

        public static TestModel CreateTestModel(Int32 id = 0)
        {
            return new TestModel
            {
                Title = "Title" + id
            };
        }

        #endregion
    }
}
