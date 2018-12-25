using Microsoft.AspNetCore.Mvc;
using UpsCoolWeb.Components.Lookups;
using UpsCoolWeb.Components.Mvc;
using UpsCoolWeb.Components.Security;
using UpsCoolWeb.Data.Core;
using UpsCoolWeb.Objects;
using NonFactors.Mvc.Lookup;
using System;

namespace UpsCoolWeb.Controllers
{
    [AllowUnauthorized]
    public class LookupController : BaseController
    {
        private IUnitOfWork UnitOfWork { get; }

        public LookupController(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        [NonAction]
        public virtual JsonResult GetData(MvcLookup lookup, LookupFilter filter)
        {
            lookup.Filter = filter;

            return Json(lookup.GetData());
        }

        [AjaxOnly]
        public JsonResult Role(LookupFilter filter)
        {
            return GetData(new MvcLookup<Role, RoleView>(UnitOfWork), filter);
        }

        protected override void Dispose(Boolean disposing)
        {
            UnitOfWork.Dispose();

            base.Dispose(disposing);
        }
    }
}
