using UpsCoolWeb.Components.Extensions;
using UpsCoolWeb.Data.Core;
using UpsCoolWeb.Objects;
using UpsCoolWeb.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UpsCoolWeb.Services
{
    public class RoleService : BaseService, IRoleService
    {
        public RoleService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public virtual void SeedPermissions(RoleView view)
        {
            MvcTreeNode root = new MvcTreeNode(Resource.ForString("All"));
            view.Permissions.Nodes.Add(root);

            foreach (IGrouping<String, Permission> area in GetAllPermissions().GroupBy(permission => permission.Area))
            {
                MvcTreeNode areaNode = new MvcTreeNode(area.Key);
                foreach (IGrouping<String, Permission> controller in area.GroupBy(permission => permission.Controller))
                {
                    MvcTreeNode controllerNode = new MvcTreeNode(controller.Key);
                    foreach (Permission permission in controller)
                        controllerNode.Children.Add(new MvcTreeNode(permission.Id, permission.Action));

                    if (areaNode.Title == null)
                        root.Children.Add(controllerNode);
                    else
                        areaNode.Children.Add(controllerNode);
                }

                if (areaNode.Title != null)
                    root.Children.Add(areaNode);
            }
        }

        public IQueryable<RoleView> GetViews()
        {
            return UnitOfWork
                .Select<Role>()
                .To<RoleView>()
                .OrderByDescending(role => role.Id);
        }
        public RoleView GetView(Int32 id)
        {
            RoleView role = UnitOfWork.GetAs<Role, RoleView>(id);
            if (role != null)
            {
                role.Permissions.SelectedIds = new HashSet<Int32>(UnitOfWork
                    .Select<RolePermission>()
                    .Where(rolePermission => rolePermission.RoleId == role.Id)
                    .Select(rolePermission => rolePermission.PermissionId));

                SeedPermissions(role);
            }

            return role;
        }

        public void Create(RoleView view)
        {
            Role role = UnitOfWork.To<Role>(view);
            foreach (Int32 permissionId in view.Permissions.SelectedIds)
                role.Permissions.Add(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId
                });

            UnitOfWork.Insert(role);
            UnitOfWork.Commit();
        }
        public void Edit(RoleView view)
        {
            List<Int32> permissions = view.Permissions.SelectedIds.ToList();
            Role role = UnitOfWork.Get<Role>(view.Id);
            role.Title = view.Title;

            foreach (RolePermission rolePermission in role.Permissions.ToArray())
                if (!permissions.Remove(rolePermission.PermissionId))
                    UnitOfWork.Delete(rolePermission);

            foreach (Int32 permissionId in permissions)
                UnitOfWork.Insert(new RolePermission { RoleId = role.Id, PermissionId = permissionId });

            UnitOfWork.Commit();
        }
        public void Delete(Int32 id)
        {
            Role role = UnitOfWork.Get<Role>(id);
            role.Accounts.ForEach(account => account.RoleId = null);

            UnitOfWork.DeleteRange(role.Permissions);
            UnitOfWork.Delete(role);
            UnitOfWork.Commit();
        }

        private IEnumerable<Permission> GetAllPermissions()
        {
            return UnitOfWork
                .Select<Permission>()
                .ToArray()
                .Select(permission => new Permission
                {
                    Id = permission.Id,
                    Area = Resource.ForPermission(permission.Area),
                    Controller = Resource.ForPermission(permission.Area, permission.Controller),
                    Action = Resource.ForPermission(permission.Area, permission.Controller, permission.Action)
                })
                .OrderBy(permission => permission.Area ?? permission.Controller)
                .ThenBy(permission => permission.Controller)
                .ThenBy(permission => permission.Action);
        }
    }
}
