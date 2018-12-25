using UpsCoolWeb.Objects;
using System;

namespace UpsCoolWeb.Validators
{
    public interface IRoleValidator : IValidator
    {
        Boolean CanCreate(RoleView view);
        Boolean CanEdit(RoleView view);
    }
}
