using Microsoft.AspNetCore.Mvc.ModelBinding;
using UpsCoolWeb.Components.Notifications;
using System;

namespace UpsCoolWeb.Validators
{
    public interface IValidator : IDisposable
    {
        ModelStateDictionary ModelState { get; set; }
        Int32 CurrentAccountId { get; set; }
        Alerts Alerts { get; set; }
    }
}
