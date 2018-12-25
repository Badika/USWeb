using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;

namespace UpsCoolWeb.Components.Security.Area.Tests
{
    [Authorize]
    [Area("Area")]
    [ExcludeFromCodeCoverage]
    public class AuthorizedController : Controller
    {
        [HttpGet]
        public ViewResult Action()
        {
            return null;
        }

        [HttpPost]
        public ViewResult Action(Object obj)
        {
            return null;
        }

        [HttpGet]
        [AllowAnonymous]
        public ViewResult AuthorizedGetAction()
        {
            return null;
        }

        [HttpPost]
        public ViewResult AuthorizedGetAction(Object obj)
        {
            return null;
        }

        [HttpPost]
        public ViewResult AuthorizedPostAction()
        {
            return null;
        }

        [HttpGet]
        [AllowAnonymous]
        [ActionName("AuthorizedNamedGetAction")]
        public ViewResult GetActionWithName()
        {
            return null;
        }

        [HttpPost]
        [ActionName("AuthorizedNamedGetAction")]
        public ViewResult GetActionWithName(Object obj)
        {
            return null;
        }

        [HttpPost]
        [ActionName("AuthorizedNamedPostAction")]
        public ViewResult PostActionWithName()
        {
            return null;
        }

        [HttpGet]
        [AuthorizeAs("Action")]
        public ViewResult AuthorizedAsAction()
        {
            return null;
        }

        [HttpGet]
        [AuthorizeAs("AuthorizedAsSelf")]
        public ViewResult AuthorizedAsSelf()
        {
            return null;
        }

        [HttpGet]
        [AuthorizeAs("InheritanceAction", Controller = "InheritedAuthorized", Area = "")]
        public ViewResult AuthorizedAsOtherAction()
        {
            return null;
        }
    }
}
