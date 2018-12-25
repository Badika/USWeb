using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using UpsCoolWeb.Components.Extensions;
using UpsCoolWeb.Components.Notifications;
using UpsCoolWeb.Components.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace UpsCoolWeb.Controllers
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public abstract class BaseController : Controller
    {
        public virtual Int32 CurrentAccountId { get; protected set; }
        public IAuthorization Authorization { get; protected set; }
        public Alerts Alerts { get; protected set; }

        protected BaseController()
        {
            Alerts = new Alerts();
        }

        public virtual ActionResult NotEmptyView(Object model)
        {
            if (model == null)
                return RedirectToNotFound();

            return View(model);
        }
        public virtual ActionResult RedirectToLocal(String url)
        {
            if (!Url.IsLocalUrl(url))
                return RedirectToDefault();

            return Redirect(url);
        }
        public virtual RedirectToActionResult RedirectToDefault()
        {
            return base.RedirectToAction("Index", "Home", new { area = "" });
        }
        public virtual RedirectToActionResult RedirectToNotFound()
        {
            return base.RedirectToAction("NotFound", "Home", new { area = "" });
        }
        public override RedirectToActionResult RedirectToAction(String action, String controller, Object route)
        {
            IDictionary<String, Object> values = HtmlHelper.AnonymousObjectToHtmlAttributes(route);
            controller = controller ?? (values.ContainsKey("controller") ? values["controller"] as String : null);
            String area = values.ContainsKey("area") ? values["area"] as String : null;
            controller = controller ?? RouteData.Values["controller"] as String;
            area = area ?? RouteData.Values["area"] as String;

            if (!IsAuthorizedFor(action, controller, area))
                return RedirectToDefault();

            return base.RedirectToAction(action, controller, route);
        }

        public virtual Boolean IsAuthorizedFor(String action, String controller, String area)
        {
            return Authorization?.IsGrantedFor(CurrentAccountId, area, controller, action) != false;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Authorization = HttpContext.RequestServices.GetService<IAuthorization>();

            CurrentAccountId = User.Id() ?? 0;
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (!(context.Result is JsonResult))
            {
                Alerts alerts = JsonConvert.DeserializeObject<Alerts>(TempData["Alerts"] as String ?? "");
                alerts = (alerts ?? Alerts);
                alerts.Merge(Alerts);

                TempData["Alerts"] = JsonConvert.SerializeObject(alerts);
            }
        }
    }
}
