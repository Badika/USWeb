using Microsoft.AspNetCore.Mvc.ModelBinding;
using UpsCoolWeb.Data.Core;
using UpsCoolWeb.Objects;
using UpsCoolWeb.Resources;
using System;
using System.Linq;

namespace UpsCoolWeb.Validators
{
    public class RoleValidator : BaseValidator, IRoleValidator
    {
        public RoleValidator(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public Boolean CanCreate(RoleView view)
        {
            Boolean isValid = ModelState.IsValid;
            isValid &= IsUniqueTitle(view);

            return isValid;
        }
        public Boolean CanEdit(RoleView view)
        {
            Boolean isValid = ModelState.IsValid;
            isValid &= IsUniqueTitle(view);

            return isValid;
        }

        private Boolean IsUniqueTitle(RoleView view)
        {
            Boolean isUnique = !UnitOfWork
                .Select<Role>()
                .Any(role =>
                    role.Id != view.Id &&
                    role.Title.ToLower() == (view.Title ?? "").ToLower());

            if (!isUnique)
                ModelState.AddModelError<RoleView>(role => role.Title,
                    Validation.For<RoleView>("UniqueTitle"));

            return isUnique;
        }
    }
}
