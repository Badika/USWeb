using Microsoft.AspNetCore.Mvc.Rendering;
using UpsCoolWeb.Components.Extensions;
using UpsCoolWeb.Components.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace UpsCoolWeb.Components.Mvc
{
    public class SiteMap : ISiteMap
    {
        private IAuthorization Authorization { get; }
        private IEnumerable<SiteMapNode> Tree { get; }
        private IEnumerable<SiteMapNode> Nodes { get; }

        public SiteMap(String map, IAuthorization authorization)
        {
            XElement siteMap = XElement.Parse(map);
            Authorization = authorization;
            Tree = Parse(siteMap);
            Nodes = Flatten(Tree);
        }

        public IEnumerable<SiteMapNode> For(ViewContext context)
        {
            Int32? account = context.HttpContext.User.Id();
            String area = context.RouteData.Values["area"] as String;
            String action = context.RouteData.Values["action"] as String;
            String controller = context.RouteData.Values["controller"] as String;
            IEnumerable<SiteMapNode> nodes = SetState(Tree, area, controller, action);

            return Authorize(account, nodes);
        }
        public IEnumerable<SiteMapNode> BreadcrumbFor(ViewContext context)
        {
            String area = context.RouteData.Values["area"] as String;
            String action = context.RouteData.Values["action"] as String;
            String controller = context.RouteData.Values["controller"] as String;

            SiteMapNode current = Nodes.SingleOrDefault(node =>
                String.Equals(node.Area, area, StringComparison.OrdinalIgnoreCase) &&
                String.Equals(node.Action, action, StringComparison.OrdinalIgnoreCase) &&
                String.Equals(node.Controller, controller, StringComparison.OrdinalIgnoreCase));

            List<SiteMapNode> breadcrumb = new List<SiteMapNode>();
            while (current != null)
            {
                breadcrumb.Insert(0, new SiteMapNode
                {
                    IconClass = current.IconClass,

                    Controller = current.Controller,
                    Action = current.Action,
                    Area = current.Area
                });

                current = current.Parent;
            }

            return breadcrumb;
        }

        private IEnumerable<SiteMapNode> SetState(IEnumerable<SiteMapNode> nodes, String area, String controller, String action)
        {
            List<SiteMapNode> copies = new List<SiteMapNode>();
            foreach (SiteMapNode node in nodes)
            {
                SiteMapNode copy = new SiteMapNode();
                copy.IconClass = node.IconClass;
                copy.IsMenu = node.IsMenu;

                copy.Controller = node.Controller;
                copy.Action = node.Action;
                copy.Area = node.Area;

                copy.Children = SetState(node.Children, area, controller, action);
                copy.HasActiveChildren = copy.Children.Any(child => child.IsActive || child.HasActiveChildren);
                copy.IsActive =
                    copy.Children.Any(child => child.IsActive && !child.IsMenu) ||
                    String.Equals(node.Area, area, StringComparison.OrdinalIgnoreCase) &&
                    String.Equals(node.Action, action, StringComparison.OrdinalIgnoreCase) &&
                    String.Equals(node.Controller, controller, StringComparison.OrdinalIgnoreCase);

                copies.Add(copy);
            }

            return copies;
        }
        private IEnumerable<SiteMapNode> Authorize(Int32? accountId, IEnumerable<SiteMapNode> nodes)
        {
            List<SiteMapNode> authorized = new List<SiteMapNode>();
            foreach (SiteMapNode node in nodes)
            {
                node.Children = Authorize(accountId, node.Children);

                if (node.IsMenu && IsAuthorizedFor(accountId, node.Area, node.Controller, node.Action) && !IsEmpty(node))
                    authorized.Add(node);
                else
                    authorized.AddRange(node.Children);
            }

            return authorized;
        }

        private Boolean IsAuthorizedFor(Int32? accountId, String area, String controller, String action)
        {
            return action == null || Authorization?.IsGrantedFor(accountId, area, controller, action) != false;
        }
        private IEnumerable<SiteMapNode> Parse(XElement root, SiteMapNode parent = null)
        {
            List<SiteMapNode> nodes = new List<SiteMapNode>();
            foreach (XElement element in root.Elements("siteMapNode"))
            {
                SiteMapNode node = new SiteMapNode();

                node.IsMenu = (Boolean?)element.Attribute("menu") == true;
                node.Controller = (String)element.Attribute("controller");
                node.IconClass = (String)element.Attribute("icon");
                node.Action = (String)element.Attribute("action");
                node.Area = (String)element.Attribute("area");
                node.Children = Parse(element, node);
                node.Parent = parent;

                nodes.Add(node);
            }

            return nodes;
        }
        private IEnumerable<SiteMapNode> Flatten(IEnumerable<SiteMapNode> branches)
        {
            List<SiteMapNode> list = new List<SiteMapNode>();
            foreach (SiteMapNode branch in branches)
            {
                list.Add(branch);
                list.AddRange(Flatten(branch.Children));
            }

            return list;
        }
        private Boolean IsEmpty(SiteMapNode node)
        {
            return node.Action == null && !node.Children.Any();
        }
    }
}
